using HazirBeton.Domain.Enums;

namespace HazirBeton.Domain.Entities;

public class SmsLog : BaseEntity
{
    public Guid ConcreteRequestId { get; set; }
    public ConcreteRequest ConcreteRequest { get; set; } = null!;

    public SmsEventType EventType { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime? SentAt { get; set; }
    public bool IsSuccessful { get; set; }
}
