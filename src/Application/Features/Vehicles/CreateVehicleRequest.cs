using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Vehicles;

public record CreateVehicleRequest(
    string Plate,
    VehicleType Type,
    DateOnly? LastMaintenanceDate,
    DateOnly? NextMaintenanceDate
);
