namespace HazirBeton.Application.Features.Auth;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task RevokeTokenAsync(string refreshToken);
}
