using Domain.Entities;
using Infrastructure.Repositories.EmailConfirmations;
using Infrastructure.Repositories.Users;

namespace Application.Services.EmailConfirmations;

public class EmailConfirmationsService(
    IUsersRepository usersRepository,
    IEmailConfirmationsRepository emailConfirmationsRepository) : IEmailConfirmationsService
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

    public Task SendEmailConfirmationLink(Guid emailConfirmationId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task ConfirmEmail(Guid emailConfirmationId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}