using HazirBeton.Application.Exceptions;
using HazirBeton.Application.Features.Sms;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HazirBeton.Infrastructure.Services.Sms;

public class SmsRetryService : ISmsRetryService
{
    private readonly AppDbContext _context;

    public SmsRetryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RetryAsync(Guid smsLogId, CancellationToken cancellationToken = default)
    {
        var entry = await _context.SmsLogs.FirstOrDefaultAsync(sl => sl.Id == smsLogId, cancellationToken);
        if (entry is null)
            throw new KeyNotFoundException($"'{smsLogId}' ID'li SMS kaydı bulunamadı.");

        if (entry.Status != SmsLogStatus.Failed)
            throw new ConflictException(
                $"Yalnızca 'Başarısız' durumundaki SMS kayıtları yeniden denenebilir. Mevcut durum: '{entry.Status}'.");

        entry.Status = SmsLogStatus.Pending;
        entry.RetryCount = 0;
        entry.NextAttemptAt = DateTime.UtcNow;
        entry.LastErrorMessage = null;
        entry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
