using Application.Extensions;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Exceptions;
using Infrastructure.Repositories.EmailConfirmations;
using Infrastructure.Repositories.Users;
using Mailer;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Services.EmailConfirmations;

public class EmailConfirmationsService(
    IUsersRepository usersRepository,
    IEmailConfirmationsRepository emailConfirmationsRepository,
    EmailService emailService,
    IOptions<AuthOptions> authOptions) : IEmailConfirmationsService
{
    private readonly AuthOptions _authOptions = authOptions.Value;

    public async Task<EmailConfirmation> CreateEmailConfirmationLink(Guid userId, CancellationToken ct)
    {
        var user = await usersRepository.GetById(userId, ct);
        if (user.IsEmailConfirmed)
            throw new ArgumentException("Email is already confirmed");

        return await CreateLink(userId, ct);
    }

    public async Task<EmailConfirmation?> CreateChangePasswordLink(string email, CancellationToken ct)
    {
        var user = await usersRepository.GetByEmail(email, ct);
        if (user is null)
            return null;

        return await CreateLink(user.Id, ct);
    }

    public async Task SendEmailConfirmationLink(Guid id, CancellationToken ct)
    {
        var emailConfirmation = await emailConfirmationsRepository.GetById(id, ct);
        var confirmationLink = $"https://fi-journal.ru/api/Users/confirmEmail/{emailConfirmation.Id}";

        await SendLink(new MailboxAddress("", emailConfirmation.User.Email), "Подтверждение Email",
            $"<p>Здравствуйте, {emailConfirmation.User.Name}!</p><p>Для подтверждения Вашего email, " +
            $"перейдите по следующей ссылке:</p><p>{confirmationLink}</p>");
    }

    public async Task SendChangePasswordLink(Guid id, CancellationToken ct)
    {
        var emailConfirmation = await emailConfirmationsRepository.GetById(id, ct);
        var link = $"https://fi-journal.ru/api/Users/changePassword/{emailConfirmation.Id}";

        await SendLink(new MailboxAddress("", emailConfirmation.User.Email), "Смена пароля",
            $"<p>Здравствуйте, {emailConfirmation.User.Name}!</p><p>Для смены Вашего пароля, " +
            $"перейдите по следующей ссылке:</p><p>{link}</p>");
    }

    public async Task ConfirmEmail(Guid id, CancellationToken ct)
    {
        try
        {
            var emailConfirmation = await emailConfirmationsRepository.GetById(id, ct);

            emailConfirmation.User.IsEmailConfirmed = true;
            await usersRepository.Update(emailConfirmation.User, ct);

            await emailConfirmationsRepository.Delete(emailConfirmation.Id, ct);
        }
        catch (InvalidOperationException e)
        {
            throw new EntityNotFoundException("Object not found", e);
        }
    }

    public async Task<string> ChangePassword(Guid id, CancellationToken ct)
    {
        try
        {
            var emailConfirmation = await emailConfirmationsRepository.GetById(id, ct);
            await emailConfirmationsRepository.Delete(emailConfirmation.Id, ct);

            return JwtProvider.GenerateToken(emailConfirmation.User.GenerateClaims(), _authOptions.JwtSecretKey, 
                _authOptions.ExpireHours);
        }
        catch (InvalidOperationException e)
        {
            throw new EntityNotFoundException("Object not found", e);
        }
    }

    private async Task<EmailConfirmation> CreateLink(Guid userId, CancellationToken ct)
    {
        var prevEmailConfirmation = await emailConfirmationsRepository.GetEmailConfirmationByUserAsync(userId, ct);
        var timeDelta = TimeSpan.FromSeconds(100);

        if (prevEmailConfirmation is not null)
            timeDelta = DateTime.UtcNow - prevEmailConfirmation.CreatedAt;

        if (timeDelta < TimeSpan.FromSeconds(60))
            throw new ArgumentException("Wait 60 seconds and try again");

        if (prevEmailConfirmation is not null)
            await emailConfirmationsRepository.Delete(prevEmailConfirmation.Id, ct);

        var emailConfirmation = new EmailConfirmation(Guid.NewGuid(), userId);
        await emailConfirmationsRepository.Add(emailConfirmation, ct);
        return emailConfirmation;
    }

    private async Task SendLink(MailboxAddress to, string subject, string text)
    {
        try
        {
            var message = new MimeMessage();
            message.To.Add(to);
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = text
            };
            await emailService.Send(message);
        }
        catch (InvalidOperationException e)
        {
            throw new EntityNotFoundException("Object not found", e);
        }
    }
}