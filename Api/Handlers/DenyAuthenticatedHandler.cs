using Microsoft.AspNetCore.Authorization;

namespace Api.Handlers;

public class DenyAuthenticatedHandler : AuthorizationHandler<DenyAuthenticatedRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        DenyAuthenticatedRequirement requirement)
    {
        if (context.User.Identity is { IsAuthenticated: true })
            context.Fail();
        else
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public class DenyAuthenticatedRequirement : IAuthorizationRequirement;