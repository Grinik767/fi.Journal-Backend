using Domain.Entities;

namespace Application.Services.UserDiffs;

public interface IUserDiffsService
{
    Task<UserDiff> Add(Dictionary<string, double> diff, Guid userId, Guid tableId, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
    Task<UserDiff> GetById(Guid id, CancellationToken ct);
    Task<List<UserDiff>> GetAll(CancellationToken ct);
    
}