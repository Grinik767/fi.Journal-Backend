using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.EmailConfirmations;

public class EmailConfirmationsRepository(JournalDbContext dbContext) : IEmailConfirmationsRepository
{
    public async Task Add(EmailConfirmation confirmation, CancellationToken ct)
    {
        await dbContext.EmailConfirmations.AddAsync(confirmation, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<EmailConfirmation> Update(EmailConfirmation confirmation, CancellationToken ct)
    {
        await dbContext.SaveChangesAsync(ct);
        return confirmation;
    }

    public async Task Delete(Guid id, CancellationToken ct) =>
        await dbContext.EmailConfirmations
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);

    public async Task<EmailConfirmation> GetById(Guid id, CancellationToken ct) =>
        await dbContext.EmailConfirmations
            .Include(e => e.User)
            .FirstAsync(e => e.Id == id, ct);

    public async Task<List<EmailConfirmation>> GetAll(CancellationToken ct) =>
        await dbContext.EmailConfirmations.AsNoTracking()
            .ToListAsync(ct);
}