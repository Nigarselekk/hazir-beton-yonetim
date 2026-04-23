namespace HazirBeton.Application.Features.Auth;

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    string Role,
    List<string> Permissions
);
