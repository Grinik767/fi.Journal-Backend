using Domain.Entities;
using Infrastructure.Exceptions;
using Infrastructure.Repositories.EmailConfirmations;
using Infrastructure.Repositories.Users;
using Mailer;
using MimeKit;

namespace Application.Services.EmailConfirmations;

public class EmailConfirmationsService(
    IUsersRepository usersRepository,
    IEmailConfirmationsRepository emailConfirmationsRepository,
    EmailService emailService) : IEmailConfirmationsService
{
    public async Task<EmailConfirmation> CreateEmailConfirmationLink(Guid userId, CancellationToken ct)
    {
        var user = await usersRepository.GetById(userId, ct);
        if (user.IsEmailConfirmed)
            throw new ArgumentException("Email is already confirmed");

        var prevEmailConfirmation = await emailConfirmationsRepository.GetEmailConfirmationByUserAsync(userId, ct);
        if (prevEmailConfirmation is not null)
            await emailConfirmationsRepository.Delete(prevEmailConfirmation.Id, ct);

        var emailConfirmation = new EmailConfirmation(Guid.NewGuid(), userId);
        await emailConfirmationsRepository.Add(emailConfirmation, ct);
        return emailConfirmation;
    }

    public async Task SendEmailConfirmationLink(Guid id, CancellationToken ct)
    {
        try
        {
            var emailConfirmation = await emailConfirmationsRepository.GetById(id, ct);
            var confirmationLink = $"https://fi-journal.ru/api/Users/confirmEmail/{emailConfirmation.Id}";

            var message = new MimeMessage();
            message.To.Add(new MailboxAddress("", emailConfirmation.User.Email));
            message.Subject = "Подтверждение Email";
            message.Body = new TextPart("html")
            {
                Text =
                    $"<p>Здравствуйте, {emailConfirmation.User.Name}!</p><p>Для подтверждения вашего email, " +
                    $"перейдите по следующей ссылке:</p><p><a href=\"{confirmationLink}\">Подтвердить Email</a></p>"
            };
            await emailService.Send(message);

            await emailConfirmationsRepository.Delete(emailConfirmation.Id, ct);
        }
        catch (InvalidOperationException e)
        {
            throw new EntityNotFoundException("Object not found", e);
        }
    }

    public Task ConfirmEmail(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}