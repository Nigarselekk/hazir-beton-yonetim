using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HazirBeton.API.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public Permission Permission { get; }

    public PermissionRequirement(Permission permission)
    {
        Permission = permission;
    }
}
