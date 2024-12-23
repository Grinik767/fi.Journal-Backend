using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.UserDiffs;

public class UserDiffsRepository(JournalDbContext dbContext) : IUserDiffsRepository
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

    public async Task Delete(Guid id, CancellationToken ct)
    {
        await dbContext.UserDiffs
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);
    }

    public async Task<UserDiff> GetById(Guid id, CancellationToken ct) =>
        await dbContext.UserDiffs.Include(u => u.User)
            .Include(t => t.Table)
            .FirstAsync(u => u.Id == id, ct);


    public async Task<List<UserDiff>> GetAll(CancellationToken ct) =>
        await dbContext.UserDiffs.AsNoTracking().Include(u => u.User)
            .Include(t => t.Table)
            .ToListAsync(ct);

    public async Task<List<UserDiff>> GetAllByUser(Guid userId, CancellationToken ct) =>
        await dbContext.UserDiffs.AsNoTracking()
            .Include(diff => diff.Table)
            .Where(diff => diff.UserId == userId)
            .GroupBy(diff => diff.TableId)
            .Select(group => group
                .OrderByDescending(diff => diff.UpdateTime)
                .First())
            .ToListAsync(ct);


    public async Task<UserDiff> GetAllByUserWithCertainTable(Guid userId, Guid tableId, CancellationToken ct) =>
        await dbContext.UserDiffs.AsNoTracking()
            .Include(diff => diff.Table)
            .Where(diff => diff.UserId == userId && diff.TableId == tableId)
            .FirstAsync(ct);
}