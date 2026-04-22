using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.VehiclePersonnel;

public record VehiclePersonnelDto(
    Guid Id,
    Guid VehicleId,
    Guid PersonnelId,
    string PersonnelFullName,
    string PersonnelPhone,
    PersonnelType PersonnelType,
    string PersonnelTypeLabel,
    PersonnelAssignmentType AssignmentType,
    string AssignmentTypeLabel,
    DateTime CreatedAt
);
