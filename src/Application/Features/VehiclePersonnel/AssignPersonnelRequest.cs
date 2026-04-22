using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.VehiclePersonnel;

public record AssignPersonnelRequest(
    Guid PersonnelId,
    PersonnelAssignmentType AssignmentType
);
