using Microsoft.AspNetCore.Authorization;

namespace HistoriasPaolin.Api.Security;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        if (context.User.Claims.Any(claim => claim.Type == "permission" && claim.Value == requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
