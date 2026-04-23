using HazirBeton.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace HazirBeton.API.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.IsInRole(nameof(UserRole.HeadManager)))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var hasPermission = context.User.Claims
            .Any(c => c.Type == "permission" && c.Value == requirement.Permission.ToString());

        if (hasPermission)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
