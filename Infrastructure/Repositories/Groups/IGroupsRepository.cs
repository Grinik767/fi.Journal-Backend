using Domain.Entities;

namespace Infrastructure.Repositories.Groups;

public interface IGroupsRepository : IRepository<Group>
{
    Task<Group?> GetById(Guid id);
}