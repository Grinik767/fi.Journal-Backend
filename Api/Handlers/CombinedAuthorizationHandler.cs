using Microsoft.AspNetCore.Authorization;

namespace Api.Handlers;

public class CombinedAuthorizationHandler(IServiceProvider serviceProvider)
    : AuthorizationHandler<CombinedAuthorizationRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        CombinedAuthorizationRequirement requirement)
    {
        var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

        var results = new List<bool>();
        foreach (var policy in requirement.Policies)
        {
            var result = await authorizationService.AuthorizeAsync(context.User, context.Resource, policy);
            results.Add(result.Succeeded);
        }

        if (results.Any(r => r))
            context.Succeed(requirement);
        else
            context.Fail();
    }
}

public class CombinedAuthorizationRequirement(string[] policies) : IAuthorizationRequirement
{
    public IEnumerable<string> Policies { get; } = policies;
}