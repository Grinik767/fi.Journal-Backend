using Domain.Entities;
using Infrastructure.Repositories.Users;

namespace Application.Services.EmailConfirmations;

public class EmailConfirmationsService(IUsersRepository usersRepository) : IEmailConfirmationsService
{
    public Task<EmailConfirmation> CreateEmailConfirmationLink(Guid userId, CancellationToken ct)
    {
        throw new NotImplementedException();
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