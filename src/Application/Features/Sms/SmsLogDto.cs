using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Sms;

public record SmsLogDto(
    Guid Id,
    SmsEventType EventType,
    string EventTypeLabel,
    SmsLogStatus Status,
    string StatusLabel,
    string Recipient,
    string Content,
    int RetryCount,
    DateTime? LastAttemptAt,
    DateTime? NextAttemptAt,
    DateTime? SentAt,
    string? LastErrorMessage,
    string? ProviderMessageId,
    DateTime CreatedAt
);
