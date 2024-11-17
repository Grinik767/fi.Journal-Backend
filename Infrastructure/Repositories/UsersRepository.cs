using Domain.Entities;
using Domain.ValueTypes;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UsersRepository(JournalDbContext dbContext) : IRepository<User>
{
    public async Task Add(User user, CancellationToken ct)
    {
        await dbContext.Users.AddAsync(user, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct) =>
        await dbContext.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);

    public async Task<User?> Update(Guid id, string? email, string? passwordHash, CancellationToken ct)
    {
        var user = await GetById(id, ct);

        if (user is null)
            return user;

        user.Email = email ?? user.Email;
        user.PasswordHash = passwordHash ?? user.PasswordHash;
        await dbContext.SaveChangesAsync(ct);

        return user;
    }

    public async Task<List<User>> GetAll(CancellationToken ct) =>
        await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Groups)
            .Include(u => u.GroupsAsAdmin)
            .ToListAsync(ct);

    public async Task<User?> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Users
            .Include(u => u.Groups)
            .Include(u => u.GroupsAsAdmin)
            .FirstOrDefaultAsync(user => user.Id == id, ct);

    public async Task<List<UserDiff>> GetUserUpdaes(Guid id, CancellationToken ct)
    {
        var listOfUserDiff = await dbContext.Users.Where(user => user.Id == id).Select(x => x.UserDiffs).FirstAsync(ct);
        return listOfUserDiff.OrderBy(x => x.UpdateTime).ToList();
    }
       
}