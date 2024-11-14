using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GroupsRepository(JournalDbContext dbContext) : IRepository<Group>
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

    public async Task<Group?> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Groups
            .Include(g => g.Admin)
            .Include(g => g.Users)
            .Include(g => g.Tables)
            .FirstOrDefaultAsync(group => group.Id == id, ct);

    public async Task<List<Group>> GetAll(CancellationToken ct) =>
        await dbContext.Groups
            .AsNoTracking()
            .Include(g => g.Admin)
            .Include(g => g.Users)
            .Include(g => g.Tables)
            .ToListAsync(ct);

    public async Task<Group?> Update(Guid id, string? name, CancellationToken ct)
    {
        var group = await GetById(id, ct);
        if (group is null)
            return group;

        group.Name = name ?? group.Name;
        await dbContext.SaveChangesAsync(ct);

        return group;
    }

    public async Task<List<User>?> GetUsers(Guid id, CancellationToken ct)
    {
        var group = await dbContext.Groups
            .AsNoTracking()
            .Include(g => g.Users)
            .FirstOrDefaultAsync(group => group.Id == id, ct);

        return group?.Users;
    }
        

    public async Task<Group> AddOrDeleteUser(Group group, User user, bool isAdd, CancellationToken ct)
    {
        if (isAdd)
            group.Users.Add(user);
        else
            group.Users.Remove(user);
        await dbContext.SaveChangesAsync(ct);

        return group;
    }
}