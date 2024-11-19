namespace Application;

public class AuthOptions
{
    public int ExpireHours { get; init; }
    public string JwtSecretKey { get; init; } = string.Empty;
    public string CookieName { get; init; } = string.Empty;
}