using System.Globalization;
using HazirBeton.Application.Features.Sms;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;

namespace HazirBeton.Infrastructure.Services.Sms;

public class SmsContentBuilder : ISmsContentBuilder
{
    private static readonly CultureInfo TurkishCulture = new("tr-TR");

    private static readonly TimeZoneInfo IstanbulTimeZone = ResolveIstanbulTimeZone();

    public string Build(ConcreteRequest request, SmsEventType eventType)
    {
        return eventType switch
        {
            SmsEventType.RequestApproved  => BuildApproval(request),
            SmsEventType.RequestDelivered => BuildDelivery(request),
            _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, "Bilinmeyen SMS olay türü.")
        };
    }

    private static string BuildApproval(ConcreteRequest r)
    {
        var appointment = r.ApprovedAppointmentDateTime ?? r.RequestedDateTime;
        var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(appointment, DateTimeKind.Utc), IstanbulTimeZone);
        var when = local.ToString("dd.MM.yyyy HH:mm", TurkishCulture);
        var qty  = r.RequestedQuantity.ToString("0.##", TurkishCulture);

        return $"Sayın {r.RequesterName}, {when} için {qty} m³ {r.MaterialType} talebiniz onaylanmıştır.";
    }

    private static string BuildDelivery(ConcreteRequest r)
    {
        var qty = (r.DeliveredQuantity ?? 0m).ToString("0.##", TurkishCulture);
        return $"Sayın {r.RequesterName}, {qty} m³ {r.MaterialType} teslimatı tamamlanmıştır. İyi günler dileriz.";
    }

    private static TimeZoneInfo ResolveIstanbulTimeZone()
    {
        // Linux/macOS use IANA names; Windows uses the legacy display name.
        foreach (var id in new[] { "Europe/Istanbul", "Turkey Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
            catch (TimeZoneNotFoundException) { }
        }
        return TimeZoneInfo.Utc;
    }
}
