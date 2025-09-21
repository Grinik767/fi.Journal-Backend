using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Groups;

public class GroupsRepository(JournalDbContext dbContext) : IGroupsRepository
{
    public async Task Add(Group group, CancellationToken ct)
    {
        await dbContext.Groups.AddAsync(group, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct) =>
        await dbContext.Groups
            .Where(g => g.Id == id)
            .ExecuteDeleteAsync(ct);

    public async Task<Group> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Groups
            .Include(g => g.Admin)
            .Include(g => g.Users)
            .Include(g => g.Tables)
            .FirstAsync(group => group.Id == id, ct);
    
    public async Task<Group?> GetByName(string groupName) =>
        await dbContext.Groups.FirstOrDefaultAsync(g => g.Name == groupName);

    public async Task<List<Group>> GetByNames(List<string> names) =>
        await dbContext.Groups.Where(g => names.Contains(g.Name)).ToListAsync();

    public async Task<Group?> GetById(Guid id) =>
        await dbContext.Groups
            .AsNoTracking()
            .Include(g => g.Users)
            .FirstOrDefaultAsync(group => group.Id == id);

    public async Task<List<Group>> GetAll(CancellationToken ct) =>
        await dbContext.Groups
            .AsNoTracking()
            .Include(g => g.Admin)
            .Include(g => g.Users)
            .Include(g => g.Tables)
            .ToListAsync(ct);

    public async Task<Group> Update(Group group, CancellationToken ct)
    {
        await dbContext.SaveChangesAsync(ct);
        return group;
    }
}