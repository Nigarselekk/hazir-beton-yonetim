namespace HazirBeton.Application.Features.Users;

public record UpdateUserRequest(
    string? FullName = null,
    string? Password = null
);
