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
                    $"<p>Здравствуйте, {emailConfirmation.User.Name}!</p><p>Для подтверждения Вашего email, " +
                    $"перейдите по следующей ссылке:</p><p>{confirmationLink}</p>"
            };
            await emailService.Send(message);
        }
        catch (InvalidOperationException e)
        {
            throw new EntityNotFoundException("Object not found", e);
        }
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
}