using Domain.Entities;

namespace Application.Services.Groups;

public interface IGroupDistributionService
{
    public Task<User> DistributeUserIntoGroups(User user, CancellationToken ct);
}