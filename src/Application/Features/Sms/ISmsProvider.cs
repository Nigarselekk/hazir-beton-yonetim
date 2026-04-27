namespace HazirBeton.Application.Features.Sms;

public interface ISmsProvider
{
    Task<SmsSendResult> SendAsync(string recipient, string content, CancellationToken cancellationToken);
}
