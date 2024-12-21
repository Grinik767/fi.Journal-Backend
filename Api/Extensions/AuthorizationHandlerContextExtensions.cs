using Microsoft.AspNetCore.Authorization;

namespace Api.Extensions;

public static class AuthorizationHandlerContextExtensions
{
    public static bool IsAuthenticated(this AuthorizationHandlerContext context) =>
        context.User.Identity is { IsAuthenticated: true };
    
    public static Guid GetUserId(this AuthorizationHandlerContext context) =>
        Guid.Parse(context.User.Claims.First(claim => claim.Type == "id").Value);
}