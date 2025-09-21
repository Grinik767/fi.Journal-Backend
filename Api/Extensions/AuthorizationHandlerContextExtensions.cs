using Microsoft.AspNetCore.Authorization;

namespace Api.Extensions;

public static class AuthorizationHandlerContextExtensions
{
    /// <summary>
    /// Проверка у пользователя аутентификации
    /// </summary>
    public static bool IsAuthenticated(this AuthorizationHandlerContext context) =>
        context.User.Identity is { IsAuthenticated: true };
    
    /// <summary>
    /// id пользователя из текущего аутентифицированного пользователя (для аутентификации)
    /// </summary>
    public static Guid GetUserId(this AuthorizationHandlerContext context) =>
        Guid.Parse(context.User.Claims.First(claim => claim.Type == "id").Value);
}