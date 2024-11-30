using Domain.Entities;

namespace Application.Services.UserDiffs;

public interface IUserDiffsService
{
    Task<List<UserDiff>> GetDiffsForUser(Guid userId, CancellationToken ct);

    Task UpdateUsersDiffs(Table table, string oldPath, string newPath, CancellationToken ct);
}