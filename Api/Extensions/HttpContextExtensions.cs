namespace Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserIdFromHttpContext(this HttpContext context) =>
        Guid.Parse(context.User.Claims.First(claim => claim.Type == "id").Value);
}