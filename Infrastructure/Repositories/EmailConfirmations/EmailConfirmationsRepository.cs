using Domain.Entities;

namespace Infrastructure.Repositories.EmailConfirmations;

public class EmailConfirmationsRepository : IEmailConfirmationsRepository
{
    public Task Add(EmailConfirmation entity, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<EmailConfirmation> Update(EmailConfirmation entity, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<EmailConfirmation> GetById(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<List<EmailConfirmation>> GetAll(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}