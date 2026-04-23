using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HazirBeton.API.Authorization;

public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(Permission permission)
        : base($"Permission:{permission}") { }
}
