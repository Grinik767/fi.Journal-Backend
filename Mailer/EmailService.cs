using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Mailer;

public class EmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService(IOptions<MailerOptions> mailerOptions)
    {
        var options = mailerOptions.Value;

        using var smtpClient = new SmtpClient(options.Host, options.Port);
        smtpClient.Credentials = new NetworkCredential(options.Email, options.Password);
        smtpClient.EnableSsl = true;

        _smtpClient = smtpClient;
    }

    public async Task Send(MailMessage mailMessage)
    {
        try
        {
            await _smtpClient.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while sending email: {ex.Message}");
            throw;
        }
    }
}