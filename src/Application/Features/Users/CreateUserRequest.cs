using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Features.Users;

public record CreateUserRequest(
    string Username,
    string FullName,
    string Password,
    UserRole Role,
    List<Permission>? CustomPermissions = null
);
