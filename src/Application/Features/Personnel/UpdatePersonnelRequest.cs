using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Personnel;

public record UpdatePersonnelRequest(
    string FullName,
    string Phone,
    PersonnelType Type
);
