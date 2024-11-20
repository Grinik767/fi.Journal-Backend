using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserDiffRepository(JournalDbContext dbContext) : IRepository<UserDiff>
{
    public async Task Add(UserDiff userDiff, CancellationToken ct)
    {
        await dbContext.UserDiffs.AddAsync(userDiff, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<UserDiff> Update(UserDiff userDiff, CancellationToken ct)
    {
        await dbContext.SaveChangesAsync(ct);
        return userDiff;
    }

    public  async Task Delete(Guid id, CancellationToken ct)
    {
        await dbContext.UserDiffs
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<UserDiff> GetById(Guid id, CancellationToken ct) => 
        await dbContext.UserDiffs.Include(u => u.User)
        .Include(t => t.Table)
        .Include(u => u.Diff)
        .FirstAsync(u => u.Id == id, ct);


    public async Task<List<UserDiff>> GetAll(CancellationToken ct) =>
        await dbContext.UserDiffs.AsNoTracking().Include(u => u.User)
            .Include(t => t.Table)
            .Include(u => u.Diff)
            .ToListAsync(ct);
}