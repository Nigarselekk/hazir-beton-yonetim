using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.ConcreteRequests;

public record ConcreteRequestListDto(
    Guid Id,
    ConcreteRequestStatus Status,
    string StatusLabel,
    Guid CustomerId,
    string CustomerCompanyName,
    Guid SiteId,
    string SiteName,
    string MaterialType,
    decimal RequestedQuantity,
    decimal TotalAmount,
    DateTime RequestedDateTime,
    DateTime? ApprovedAppointmentDateTime,
    List<string> AssignedVehiclePlates,
    DateTime CreatedAt,
    uint RowVersion
);
