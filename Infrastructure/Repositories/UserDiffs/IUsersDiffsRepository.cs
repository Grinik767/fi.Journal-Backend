using Domain.Entities;

namespace Infrastructure.Repositories.UserDiffs;

public interface IUsersDiffsRepository : IRepository<UserDiff>
{
    Task<List<UserDiff>> GetAllByUser(Guid userId, CancellationToken ct);
    Task<UserDiff> GetAllByUserWithCertainTable(Guid userId, Guid tableId, CancellationToken ct);
}