using Domain.Entities;

namespace Application.Services.Groups;

public interface IGroupsService
{
    Task<Group> Add(string name, Guid adminId, CancellationToken ct);
    Task<Group> Update(Guid id, string? name, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
    Task<List<Group>> GetAll(CancellationToken ct);
    Task<Group> GetById(Guid id, CancellationToken ct);
    Task<List<User>> GetUsers(Guid id, CancellationToken ct);
    Task<Group> AddUser(Guid id, Guid userId, CancellationToken ct);
    Task<Group> DeleteUser(Guid id, Guid userId, CancellationToken ct);
}