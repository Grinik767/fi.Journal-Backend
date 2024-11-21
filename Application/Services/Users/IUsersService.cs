using Domain.Entities;

namespace Application.Services.Users;

public interface IUsersService
{
    Task<User> Register(string name, string email, string password, CancellationToken ct);
    Task<string> Login(string email, string password, CancellationToken ct);
    Task<User> Update(Guid id, string? email, string? password, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
    Task<List<User>> GetAll(CancellationToken ct);
    Task<User> GetById(Guid id, CancellationToken ct);
    Task<List<Group>> GetGroups(Guid id, CancellationToken ct);
    Task<List<Group>> GetGroupsAsAdmin(Guid id, CancellationToken ct);
    Task<Dictionary<Guid, Dictionary<string, double>>> GetUserRecentPoints(Guid id, CancellationToken ct);
}