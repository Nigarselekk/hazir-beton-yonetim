using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

public class SmsLog : BaseEntity
{
    public Guid ConcreteRequestId { get; set; }
    public ConcreteRequest ConcreteRequest { get; set; } = null!;

    public SmsEventType EventType { get; set; }
    public SmsLogStatus Status { get; set; } = SmsLogStatus.Pending;

    public string Recipient { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public int RetryCount { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public DateTime? NextAttemptAt { get; set; }
    public DateTime? SentAt { get; set; }

    public string? LastErrorMessage { get; set; }
    public string? ProviderMessageId { get; set; }
}
