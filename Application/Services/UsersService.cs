using Infrastructure.Repositories;
using Domain.Entities;
using FluentValidation;

namespace Application.Services;

public class UsersService(IRepository<User> repository, IValidator<User> validator)
    : BaseService<User>(repository, validator)
{
    public async Task<User> Add(string name, string email, string password, CancellationToken ct) =>
        await base.Add(new User(Guid.NewGuid(), name, email, password), ct);

    public async Task<User> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await GetById(id, ct);
        user.Email = email ?? user.Email;
        user.PasswordHash = password ?? user.PasswordHash;

        return await base.Update(user, ct);
    }
}