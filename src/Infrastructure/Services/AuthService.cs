using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HazirBeton.Application.Features.Auth;
using HazirBeton.Domain.Entities;
using HazirBeton.Domain.Enums;
using HazirBeton.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HazirBeton.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Kullanıcı adı veya şifre hatalı.");

        return await IssueTokensAsync(user);
    }

    public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var user = await _db.Users
            .Include(u => u.Permissions)
            .FirstOrDefaultAsync(u =>
                u.RefreshToken == request.RefreshToken &&
                u.RefreshTokenExpiry > DateTime.UtcNow &&
                u.IsActive);

        if (user is null)
            throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş refresh token.");

        return await IssueTokensAsync(user);
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        if (user is null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    private async Task<TokenResponse> IssueTokensAsync(User user)
    {
        var permissions = user.Role == UserRole.HeadManager
            ? Enum.GetValues<Permission>().Select(p => p.ToString()).ToList()
            : user.Permissions.Select(p => p.Permission.ToString()).ToList();

        var expiry = DateTime.UtcNow.AddMinutes(GetAccessTokenExpiryMinutes());
        var accessToken = GenerateAccessToken(user, permissions, expiry);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(GetRefreshTokenExpiryDays());
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return new TokenResponse(accessToken, refreshToken, expiry, user.Role.ToString(), permissions);
    }

    private string GenerateAccessToken(User user, List<string> permissions, DateTime expiry)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new("username", user.Username),
            new("fullName", user.FullName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        foreach (var permission in permissions)
            claims.Add(new Claim("permission", permission));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetJwtKey()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: GetIssuer(),
            audience: GetAudience(),
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private string GetJwtKey() => _config["Jwt:Key"]
        ?? throw new InvalidOperationException("Jwt:Key is not configured.");

    private string GetIssuer() => _config["Jwt:Issuer"] ?? "hazir-beton-api";
    private string GetAudience() => _config["Jwt:Audience"] ?? "hazir-beton-clients";

    private int GetAccessTokenExpiryMinutes() =>
        int.TryParse(_config["Jwt:AccessTokenExpiryMinutes"], out var v) ? v : 60;

    private int GetRefreshTokenExpiryDays() =>
        int.TryParse(_config["Jwt:RefreshTokenExpiryDays"], out var v) ? v : 7;
}
