using Api.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Api.Handlers;

public class PersonalDataAccessHandler : AuthorizationHandler<PersonalDataAccessRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PersonalDataAccessRequirement requirement)
    {
        if (!context.IsAuthenticated() || context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (!httpContext.Request.RouteValues.TryGetValue("userId", out var userId))
            userId = httpContext.Request.RouteValues["id"];

        if (userId?.ToString() == context.GetUserId().ToString())
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}

public class PersonalDataAccessRequirement : IAuthorizationRequirement;