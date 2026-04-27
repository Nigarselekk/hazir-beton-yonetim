using HazirBeton.Application.Features.Sms;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HazirBeton.Infrastructure.Services.Sms;

// Assumes a single API instance. The batch query has no row-level claim
// (no FOR UPDATE SKIP LOCKED, no ClaimedAt/ClaimedBy columns), so two
// workers running concurrently would both pick up the same Pending row
// and dispatch the SMS twice. Before scaling the API horizontally, add a
// claim mechanism (Postgres advisory locks, SKIP LOCKED on the SELECT, or
// a ClaimedBy/ClaimedUntil pair on SmsLog) — that's the place to start.
public class SmsDispatchWorker : BackgroundService
{
    // Backoff schedule (1m, 5m, 30m, 2h, 6h). Index = RetryCount before this attempt.
    private static readonly TimeSpan[] BackoffSchedule =
    {
        TimeSpan.FromMinutes(1),
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(30),
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(6)
    };

    private readonly IServiceProvider _services;
    private readonly IOptionsMonitor<SmsOptions> _optionsMonitor;
    private readonly ILogger<SmsDispatchWorker> _logger;

    public SmsDispatchWorker(
        IServiceProvider services,
        IOptionsMonitor<SmsOptions> optionsMonitor,
        ILogger<SmsDispatchWorker> logger)
    {
        _services = services;
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SmsDispatchWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var options = _optionsMonitor.CurrentValue;
            var pollInterval = TimeSpan.FromSeconds(Math.Max(1, options.PollIntervalSeconds));

            try
            {
                if (options.Enabled)
                    await ProcessBatchAsync(options, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SmsDispatchWorker tick failed.");
            }

            try
            {
                await Task.Delay(pollInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("SmsDispatchWorker stopping.");
    }

    private async Task ProcessBatchAsync(SmsOptions options, CancellationToken cancellationToken)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var provider = scope.ServiceProvider.GetRequiredService<ISmsProvider>();

        var now = DateTime.UtcNow;
        var batchSize = Math.Clamp(options.BatchSize, 1, 200);

        var batch = await db.SmsLogs
            .Include(sl => sl.ConcreteRequest)
            .Where(sl => sl.Status == SmsLogStatus.Pending
                         && (sl.NextAttemptAt == null || sl.NextAttemptAt <= now))
            .OrderBy(sl => sl.NextAttemptAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (batch.Count == 0)
            return;

        foreach (var entry in batch)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await DispatchSingleAsync(db, provider, entry, options, cancellationToken);
        }
    }

    private async Task DispatchSingleAsync(
        AppDbContext db,
        ISmsProvider provider,
        SmsLog entry,
        SmsOptions options,
        CancellationToken cancellationToken)
    {
        var attemptStart = DateTime.UtcNow;

        // Approval SMS expires once the appointment time has passed — no point waking
        // the customer up for a meeting that already happened.
        if (entry.EventType == SmsEventType.RequestApproved && IsAppointmentExpired(entry, attemptStart))
        {
            MarkPermanentFailure(entry, attemptStart, "Randevu saati geçtiği için SMS gönderilmedi.");
            await SaveSafelyAsync(db, entry, cancellationToken);
            return;
        }

        if (string.IsNullOrWhiteSpace(entry.Recipient))
        {
            MarkPermanentFailure(entry, attemptStart, "Geçersiz telefon numarası");
            await SaveSafelyAsync(db, entry, cancellationToken);
            return;
        }

        try
        {
            SmsSendResult result;

            if (options.DryRun)
            {
                _logger.LogInformation(
                    "[Sms DryRun] Skipped send for SmsLog {Id} (event={Event}, recipient={Recipient}).",
                    entry.Id, entry.EventType, MaskRecipient(entry.Recipient));
                result = SmsSendResult.Ok($"dryrun-{Guid.NewGuid():N}");
            }
            else
            {
                result = await provider.SendAsync(entry.Recipient, entry.Content, cancellationToken);
            }

            entry.LastAttemptAt = attemptStart;

            if (result.Success)
            {
                entry.Status = SmsLogStatus.Sent;
                entry.SentAt = DateTime.UtcNow;
                entry.ProviderMessageId = result.ProviderMessageId;
                entry.LastErrorMessage = null;
                entry.NextAttemptAt = null;
                entry.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                entry.RetryCount += 1;
                entry.LastErrorMessage = Truncate(result.Error, 1000);

                var canRetry = result.IsTransient && entry.RetryCount < options.MaxAttempts;
                if (canRetry)
                {
                    entry.Status = SmsLogStatus.Pending;
                    entry.NextAttemptAt = DateTime.UtcNow + GetBackoff(entry.RetryCount);
                }
                else
                {
                    entry.Status = SmsLogStatus.Failed;
                    entry.NextAttemptAt = null;
                }
                entry.UpdatedAt = DateTime.UtcNow;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SmsDispatchWorker provider call threw for SmsLog {Id}.", entry.Id);
            entry.LastAttemptAt = attemptStart;
            entry.RetryCount += 1;
            entry.LastErrorMessage = Truncate(ex.Message, 1000);

            if (entry.RetryCount < options.MaxAttempts)
            {
                entry.Status = SmsLogStatus.Pending;
                entry.NextAttemptAt = DateTime.UtcNow + GetBackoff(entry.RetryCount);
            }
            else
            {
                entry.Status = SmsLogStatus.Failed;
                entry.NextAttemptAt = null;
            }
            entry.UpdatedAt = DateTime.UtcNow;
        }

        await SaveSafelyAsync(db, entry, cancellationToken);
    }

    private static bool IsAppointmentExpired(SmsLog entry, DateTime nowUtc)
    {
        var appointment = entry.ConcreteRequest?.ApprovedAppointmentDateTime
                          ?? entry.ConcreteRequest?.RequestedDateTime;
        return appointment.HasValue && appointment.Value < nowUtc;
    }

    private static void MarkPermanentFailure(SmsLog entry, DateTime nowUtc, string error)
    {
        entry.Status = SmsLogStatus.Failed;
        entry.LastAttemptAt = nowUtc;
        entry.NextAttemptAt = null;
        entry.LastErrorMessage = error;
        entry.UpdatedAt = nowUtc;
    }

    private async Task SaveSafelyAsync(AppDbContext db, SmsLog entry, CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to persist SmsLog state for {Id}.", entry.Id);
            // Detach so a corrupted change tracker doesn't poison the next iteration.
            db.Entry(entry).State = EntityState.Detached;
        }
    }

    private static TimeSpan GetBackoff(int retryCount)
    {
        var idx = Math.Clamp(retryCount - 1, 0, BackoffSchedule.Length - 1);
        return BackoffSchedule[idx];
    }

    private static string? Truncate(string? value, int max)
    {
        if (value is null) return null;
        return value.Length <= max ? value : value[..max];
    }

    private static string MaskRecipient(string recipient)
    {
        if (string.IsNullOrEmpty(recipient) || recipient.Length < 4)
            return "***";
        return string.Concat(recipient.AsSpan(0, recipient.Length - 4), "****");
    }
}
