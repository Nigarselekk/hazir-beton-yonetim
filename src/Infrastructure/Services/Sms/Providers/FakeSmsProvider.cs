using HazirBeton.Application.Features.Sms;
using Microsoft.Extensions.Logging;

namespace HazirBeton.Infrastructure.Services.Sms.Providers;

public class FakeSmsProvider : ISmsProvider
{
    private readonly ILogger<FakeSmsProvider> _logger;

    public FakeSmsProvider(ILogger<FakeSmsProvider> logger)
    {
        _logger = logger;
    }

    public Task<SmsSendResult> SendAsync(string recipient, string content, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[FakeSmsProvider] To {Recipient}: {Content}", MaskRecipient(recipient), content);
        return Task.FromResult(SmsSendResult.Ok($"fake-{Guid.NewGuid():N}"));
    }

    private static string MaskRecipient(string recipient)
    {
        if (string.IsNullOrEmpty(recipient) || recipient.Length < 4)
            return "***";
        return string.Concat(recipient.AsSpan(0, recipient.Length - 4), "****");
    }
}
