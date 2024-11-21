using Domain.Entities;

namespace Application.Services.UserDiffs;

public interface IUserDiffsService
{
    Task<List<UserDiff>> GetDiffForUser(Guid userId, CancellationToken ct);
}