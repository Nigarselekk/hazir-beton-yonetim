namespace HazirBeton.Application.Features.Users;

public record UserDto(
    Guid Id,
    string Username,
    string FullName,
    string Role,
    bool IsActive,
    List<string> Permissions,
    DateTime CreatedAt
);
