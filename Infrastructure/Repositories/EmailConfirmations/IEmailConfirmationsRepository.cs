using Domain.Entities;

namespace Infrastructure.Repositories.EmailConfirmations;

public interface IEmailConfirmationsRepository : IRepository<EmailConfirmation>
{
    Task<EmailConfirmation?> GetEmailConfirmationByUserAsync(Guid userId, CancellationToken ct);
}