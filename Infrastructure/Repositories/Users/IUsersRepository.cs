using Domain.Entities;

namespace Infrastructure.Repositories.Users;

public interface IUsersRepository : IRepository<User>
{
    Task<User?> GetByEmail(string email, CancellationToken ct);

    Task<bool> IsSuperAdmin(Guid id);
}