using HazirBeton.Application.Features.Sms;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;

namespace HazirBeton.Infrastructure.Services.Sms;

internal static class SmsLogMapper
{
    public static SmsLogDto ToDto(SmsLog sl) => new(
        sl.Id,
        sl.EventType,
        GetEventTypeLabel(sl.EventType),
        sl.Status,
        GetStatusLabel(sl.Status),
        sl.Recipient,
        sl.Content,
        sl.RetryCount,
        sl.LastAttemptAt,
        sl.NextAttemptAt,
        sl.SentAt,
        sl.LastErrorMessage,
        sl.ProviderMessageId,
        sl.CreatedAt);

    public static string GetEventTypeLabel(SmsEventType type) => type switch
    {
        SmsEventType.RequestApproved  => "Onay SMS",
        SmsEventType.RequestDelivered => "Teslim SMS",
        _                             => type.ToString()
    };

    public static string GetStatusLabel(SmsLogStatus status) => status switch
    {
        SmsLogStatus.Pending => "Bekliyor",
        SmsLogStatus.Sent    => "Gönderildi",
        SmsLogStatus.Failed  => "Başarısız",
        _                    => status.ToString()
    };
}
