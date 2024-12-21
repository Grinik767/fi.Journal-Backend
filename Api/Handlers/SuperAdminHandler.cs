using Api.Extensions;
using Infrastructure.Repositories.Users;
using Microsoft.AspNetCore.Authorization;

namespace Api.Handlers;

public class SuperAdminHandler(IUsersRepository usersRepository) : AuthorizationHandler<SuperAdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        SuperAdminRequirement requirement)
    {
        if (!context.IsAuthenticated())
        {
            context.Fail();
            return;
        }

        var isSuperAdmin = await usersRepository.IsSuperAdmin(context.GetUserId());
        if (isSuperAdmin)
            context.Succeed(requirement);
        else
            context.Fail();
    }
}

public class SuperAdminRequirement : IAuthorizationRequirement;