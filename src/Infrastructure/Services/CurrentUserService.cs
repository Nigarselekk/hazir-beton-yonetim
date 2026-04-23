using System.Security.Claims;
using HazirBeton.Application.Common;
using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace HazirBeton.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public Guid UserId
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string Username => Principal?.FindFirstValue("username") ?? string.Empty;

    public UserRole Role
    {
        get
        {
            var value = Principal?.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(value, out var role) ? role : UserRole.Viewer;
        }
    }

    public bool IsHeadManager => Role == UserRole.HeadManager;

    public bool HasPermission(Permission permission) =>
        IsHeadManager ||
        Principal?.Claims.Any(c => c.Type == "permission" && c.Value == permission.ToString()) == true;
}
