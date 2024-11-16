using Infrastructure.Repositories;
using Domain.Entities;
using Application.Extensions;
using FluentValidation;

namespace Application.Services;

public class UsersService(IRepository<User> repository, IValidator<User> validator)
{
    public async Task<User> Add(string name, string email, string password, CancellationToken ct)
    {
        var user = await new User(Guid.NewGuid(), name, email, password).ValidateAsync(validator, ct);

        await repository.Add(user, ct);
        return user;
    }

    public async Task Delete(Guid id, CancellationToken ct) => await repository.Delete(id, ct);

    public async Task<User> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await GetById(id, ct);
        user.Email = email ?? user.Email;
        user.PasswordHash = password ?? user.PasswordHash;
        
        await user.ValidateAsync(validator, ct);
        return await repository.Update(user, ct);
    }
    
    public async Task<List<User>> GetAll(CancellationToken ct) => await repository.GetAll(ct);

    public async Task<User> GetById(Guid id, CancellationToken ct) => await repository.GetById(id, ct);
}