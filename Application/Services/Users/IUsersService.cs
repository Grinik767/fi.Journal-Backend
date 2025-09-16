using Domain.Entities;

namespace Application.Services.Users;

public interface IUsersService
{
    Task<User> Register(string name, string email, string password, string studyGroup, CancellationToken ct);
    Task<string> Login(string email, string password, bool remember, CancellationToken ct);
    Task<User> Update(Guid id, string? email, string? password, string? studyGroup, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
    Task<List<User>> GetAll(CancellationToken ct);
    Task<User> GetById(Guid id, CancellationToken ct);
}