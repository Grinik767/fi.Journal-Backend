using Api.Extensions;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Api.Handlers;

public class UserIsGroupMemberHandler(GroupsRepository groupsRepository)
    : AuthorizationHandler<UserIsGroupMemberRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        UserIsGroupMemberRequirement requirement)
    {
        if (!context.IsAuthenticated() || context.Resource is not HttpContext httpContext ||
            !Guid.TryParse(httpContext.Request.RouteValues["id"]?.ToString(), out var groupId))
        {
            context.Fail();
            return;
        }

        var group = await groupsRepository.GetById(groupId);

        var userId = context.GetUserId();
        if (group is not null && group.Users.Any(u => u.Id == userId))
            context.Succeed(requirement);
    }
}

public class UserIsGroupMemberRequirement : IAuthorizationRequirement;