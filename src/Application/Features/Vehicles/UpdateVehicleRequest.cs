using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Vehicles;

public record UpdateVehicleRequest(
    string Plate,
    VehicleType Type,
    VehicleStatus Status,
    DateOnly? LastMaintenanceDate,
    DateOnly? NextMaintenanceDate
);
