using System.Net;
using HazirBeton.Application.Features.Sms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HazirBeton.Infrastructure.Services.Sms.Providers;

// Netgsm "get/bulkhttppost.asp" style endpoint. Real credentials and the precise
// endpoint are config-driven; this adapter never contains hard-coded secrets.
public class NetgsmSmsProvider : ISmsProvider
{
    private readonly HttpClient _httpClient;
    private readonly NetgsmOptions _options;
    private readonly ILogger<NetgsmSmsProvider> _logger;

    public NetgsmSmsProvider(
        HttpClient httpClient,
        IOptions<SmsOptions> options,
        ILogger<NetgsmSmsProvider> logger)
    {
        _httpClient = httpClient;
        _options = options.Value.Netgsm;
        _logger = logger;
    }

    public async Task<SmsSendResult> SendAsync(string recipient, string content, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl) ||
            string.IsNullOrWhiteSpace(_options.Username) ||
            string.IsNullOrWhiteSpace(_options.Password) ||
            string.IsNullOrWhiteSpace(_options.Header))
        {
            // Misconfiguration is permanent until ops fixes it; do not retry forever.
            return SmsSendResult.PermanentFailure("Netgsm yapılandırması eksik.");
        }

        var query = new Dictionary<string, string>
        {
            ["usercode"] = _options.Username,
            ["password"] = _options.Password,
            ["gsmno"]    = recipient,
            ["message"]  = content,
            ["msgheader"] = _options.Header
        };

        var url = BuildUrl(_options.BaseUrl, query);

        try
        {
            using var response = await _httpClient.GetAsync(url, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var transient = IsTransientHttpStatus(response.StatusCode);
                var msg = $"Netgsm HTTP {(int)response.StatusCode}: {Truncate(body, 200)}";
                return transient ? SmsSendResult.TransientFailure(msg) : SmsSendResult.PermanentFailure(msg);
            }

            // Netgsm responses look like "00 <bulkid>" on success. Anything else is a coded failure.
            var trimmed = body.Trim();
            var firstToken = trimmed.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;

            return firstToken switch
            {
                "00" or "01" or "02" => SmsSendResult.Ok(trimmed),
                "20" or "30" or "40" or "50" or "70" or "80" or "85" => SmsSendResult.PermanentFailure($"Netgsm hata kodu: {trimmed}"),
                _ => SmsSendResult.TransientFailure($"Netgsm beklenmeyen yanıt: {Truncate(trimmed, 200)}")
            };
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Netgsm timeout");
            return SmsSendResult.TransientFailure("Netgsm zaman aşımı.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Netgsm HTTP error");
            return SmsSendResult.TransientFailure($"Netgsm bağlantı hatası: {ex.Message}");
        }
    }

    private static bool IsTransientHttpStatus(HttpStatusCode code) =>
        (int)code is 408 or 429 or >= 500;

    private static string BuildUrl(string baseUrl, Dictionary<string, string> query)
    {
        var separator = baseUrl.Contains('?') ? '&' : '?';
        var encoded = string.Join('&', query.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
        return $"{baseUrl}{separator}{encoded}";
    }

    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max];
}
