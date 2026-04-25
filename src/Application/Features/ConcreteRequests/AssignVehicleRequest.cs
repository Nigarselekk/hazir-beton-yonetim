namespace HazirBeton.Application.Features.ConcreteRequests;

public record AssignVehicleRequest(
    Guid VehicleId,
    uint RowVersion
);
