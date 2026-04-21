using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Personnel;

public record CreatePersonnelRequest(
    string FullName,
    string Phone,
    PersonnelType Type
);
