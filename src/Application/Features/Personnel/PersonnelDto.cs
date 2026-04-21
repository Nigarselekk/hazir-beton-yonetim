using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Personnel;

public record PersonnelDto(
    Guid Id,
    string FullName,
    string Phone,
    PersonnelType Type,
    string TypeLabel,
    bool IsActive,
    DateTime CreatedAt
);
