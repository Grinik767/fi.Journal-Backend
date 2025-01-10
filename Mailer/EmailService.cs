using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

namespace Mailer;

public class EmailService(IOptions<MailerOptions> mailerOptions)
{
    private readonly MailerOptions _options = mailerOptions.Value;

    public async Task Send(MimeMessage message)
    {
        using var smtpClient = new SmtpClient();

        try
        {
            await smtpClient.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.SslOnConnect);
            await smtpClient.AuthenticateAsync(_options.Email, _options.Password);
            
            message.From.Add(new MailboxAddress("fi-journal.ru", _options.Email));
            await smtpClient.SendAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while sending email: {ex.Message}");
            throw;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
        }
    }
}