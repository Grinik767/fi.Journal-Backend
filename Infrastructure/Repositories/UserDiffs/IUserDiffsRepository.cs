using Domain.Entities;

namespace Infrastructure.Repositories.UserDiffs;

public interface IUserDiffsRepository : IRepository<UserDiff>
{
    Task<List<UserDiff>> GetAllByUser(Guid userId, CancellationToken ct);
    Task<List<UserDiff>> GetOldDiffsByUser(Guid userId, CancellationToken ct);
}