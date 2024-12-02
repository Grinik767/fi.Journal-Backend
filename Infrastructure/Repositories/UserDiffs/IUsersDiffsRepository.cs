using Domain.Entities;

namespace Infrastructure.Repositories.UserDiffs;

public interface IUsersDiffsRepository : IRepository<UserDiff>
{
    Task<List<UserDiff>> GetAllByUser(Guid userId, CancellationToken ct);
    Task<List<UserDiff>> GetAllByUserWithCertainTable(Guid userId, Guid tableId, CancellationToken ct);
}