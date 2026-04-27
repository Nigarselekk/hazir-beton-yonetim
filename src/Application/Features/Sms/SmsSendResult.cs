namespace HazirBeton.Application.Features.Sms;

public record SmsSendResult(
    bool Success,
    string? ProviderMessageId,
    string? Error,
    bool IsTransient
)
{
    public static SmsSendResult Ok(string? providerMessageId)
        => new(true, providerMessageId, null, false);

    public static SmsSendResult TransientFailure(string error)
        => new(false, null, error, true);

    public static SmsSendResult PermanentFailure(string error)
        => new(false, null, error, false);
}
