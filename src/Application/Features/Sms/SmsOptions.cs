namespace HazirBeton.Application.Features.Sms;

public class SmsOptions
{
    public const string SectionName = "Sms";

    public bool Enabled { get; set; } = true;
    public string Provider { get; set; } = "Fake";
    public bool DryRun { get; set; }
    public int MaxAttempts { get; set; } = 5;
    public int PollIntervalSeconds { get; set; } = 10;
    public int BatchSize { get; set; } = 20;

    public NetgsmOptions Netgsm { get; set; } = new();
}

public class NetgsmOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Header { get; set; } = string.Empty;
}
