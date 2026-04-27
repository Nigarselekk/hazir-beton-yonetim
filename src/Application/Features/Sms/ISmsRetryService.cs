namespace HazirBeton.Application.Features.Sms;

public interface ISmsRetryService
{
    Task RetryAsync(Guid smsLogId, CancellationToken cancellationToken = default);
}
