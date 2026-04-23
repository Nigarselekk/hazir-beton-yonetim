using HazirBeton.Domain.Enums;

namespace HazirBeton.Application.Common;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string Username { get; }
    UserRole Role { get; }
    bool IsHeadManager { get; }
    bool HasPermission(Permission permission);
}
