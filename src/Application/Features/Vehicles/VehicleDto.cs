using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Vehicles;

public record VehicleDto(
    Guid Id,
    string Plate,
    VehicleType Type,
    string TypeLabel,
    VehicleStatus Status,
    string StatusLabel,
    DateOnly? LastMaintenanceDate,
    DateOnly? NextMaintenanceDate,
    bool MaintenanceAlert,
    bool IsOverdue,
    DateTime CreatedAt
);
