using HazirBeton.Application.Features.Sms;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services.Sms;

public class SmsOutboxEnqueuer : ISmsOutboxEnqueuer
{
    private readonly AppDbContext _context;
    private readonly IPhoneNumberNormalizer _normalizer;
    private readonly ISmsContentBuilder _contentBuilder;

    public SmsOutboxEnqueuer(
        AppDbContext context,
        IPhoneNumberNormalizer normalizer,
        ISmsContentBuilder contentBuilder)
    {
        _context = context;
        _normalizer = normalizer;
        _contentBuilder = contentBuilder;
    }

    public async Task EnqueueAsync(
        ConcreteRequest request,
        SmsEventType eventType,
        CancellationToken cancellationToken = default)
    {
        // Unique index (ConcreteRequestId, EventType) is the source of truth, but we
        // also short-circuit here to keep the same DbContext from staging a duplicate
        // insert that would only fail at SaveChanges and roll back the business write.
        var alreadyQueued = await _context.SmsLogs
            .AsNoTracking()
            .AnyAsync(sl => sl.ConcreteRequestId == request.Id && sl.EventType == eventType, cancellationToken);

        if (alreadyQueued)
            return;

        var rawRecipient = request.CompanyContactPhone;
        var normalized = _normalizer.Normalize(rawRecipient);
        var content = _contentBuilder.Build(request, eventType);
        var now = DateTime.UtcNow;

        var entry = new SmsLog
        {
            Id = Guid.NewGuid(),
            ConcreteRequestId = request.Id,
            EventType = eventType,
            Content = content,
            CreatedAt = now
        };

        if (normalized is null)
        {
            // No retry path can fix a malformed source number — record the failure
            // for visibility and let operations fix the request data, not the SMS row.
            entry.Recipient = string.IsNullOrWhiteSpace(rawRecipient) ? string.Empty : rawRecipient.Trim();
            entry.Status = SmsLogStatus.Failed;
            entry.LastErrorMessage = "Geçersiz telefon numarası";
            entry.LastAttemptAt = now;
        }
        else
        {
            entry.Recipient = normalized;
            entry.Status = SmsLogStatus.Pending;
            entry.NextAttemptAt = now;
        }

        await _context.SmsLogs.AddAsync(entry, cancellationToken);
    }
}
