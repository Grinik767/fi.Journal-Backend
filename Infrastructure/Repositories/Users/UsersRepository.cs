using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Users;

public class UsersRepository(JournalDbContext dbContext) : IUsersRepository
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

    public async Task<User> Update(User user, CancellationToken ct)
    {
        await dbContext.SaveChangesAsync(ct);
        return user;
    }

    public async Task<List<User>> GetAll(CancellationToken ct) =>
        await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Groups)
            .ToListAsync(ct);

    public async Task<User> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Users
            .Include(u => u.Groups)
            .FirstAsync(user => user.Id == id, ct);

    public async Task<User?> GetByEmail(string email, CancellationToken ct) =>
        await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> IsSuperAdmin(Guid id) =>
        await dbContext.SuperAdmins.AsNoTracking()
            .AnyAsync(s => s.UserId == id);

    public async Task<bool> IsEmailConfirmed(Guid id, CancellationToken ct)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstAsync(user => user.Id == id, ct);
        return user.IsEmailConfirmed;
    }
}