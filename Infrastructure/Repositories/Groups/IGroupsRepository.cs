using Domain.Entities;

namespace Infrastructure.Repositories.Groups;

public interface IGroupsRepository : IRepository<Group>
{
    Task<Group?> GetById(Guid id);
    Task<Group?> GetByName(string groupName);
    Task<List<Group>> GetByNames(List<string> groupNames);
}