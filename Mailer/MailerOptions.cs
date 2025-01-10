namespace Mailer;

public class MailerOptions
{
    public string Host { get; init; } = string.Empty;
    public int Port { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}