using Infrastructure.Repositories;
using Domain.Entities;

namespace Application.Services;

public class UsersService(UsersRepository repository)
{
    public async Task<User> Add(string name, string email, string password, CancellationToken ct)
    {
        var user = new User(Guid.NewGuid(), name, email, password);
        await repository.Add(user, ct);
        
        return user;
    }

    public async Task Delete(Guid id, CancellationToken ct) => await repository.Delete(id, ct);

    public async Task<User?> Update(Guid id, string? email, string? password, CancellationToken ct) =>
        await repository.Update(id, email, password, ct);

    public async Task<List<User>> GetAll(CancellationToken ct) => await repository.GetAll(ct);

    public async Task<User?> GetById(Guid id, CancellationToken ct) => await repository.GetById(id, ct);
}