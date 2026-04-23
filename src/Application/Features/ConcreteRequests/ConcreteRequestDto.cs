using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.ConcreteRequests;

public record ConcreteRequestDto(
    Guid Id,
    ConcreteRequestStatus Status,
    string StatusLabel,
    Guid CustomerId,
    string CustomerCompanyName,
    Guid SiteId,
    string SiteName,
    string? SiteAddress,
    string RequesterName,
    string CompanyContactPhone,
    string SiteContactPhone,
    string MaterialType,
    decimal RequestedQuantity,
    decimal UnitPrice,
    decimal TotalAmount,
    string WaybillType,
    string DeliveryMethod,
    DateTime RequestedDateTime,
    DateTime? ApprovedAppointmentDateTime,
    decimal? DeliveredQuantity,
    string? Note,
    string? CancellationReason,
    List<AssignedVehicleDto> Vehicles,
    ConcreteRequestUserDto? CreatedBy,
    ConcreteRequestUserDto? ApprovedBy,
    ConcreteRequestUserDto? AssignedBy,
    DateTime CreatedAt
);

public record AssignedVehicleDto(
    Guid VehicleId,
    string Plate,
    VehicleType Type,
    string TypeLabel,
    VehicleStatus Status,
    string StatusLabel,
    bool MaintenanceWarning,
    List<AssignedPersonnelDto> Personnel
);

public record AssignedPersonnelDto(
    Guid PersonnelId,
    string FullName,
    string Phone,
    PersonnelType Type,
    string TypeLabel,
    PersonnelAssignmentType AssignmentType,
    string AssignmentTypeLabel
);

public record ConcreteRequestUserDto(
    Guid Id,
    string FullName
);
