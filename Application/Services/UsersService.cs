using Infrastructure.Repositories;
using Domain.Entities;
using Application.Extensions;
using FluentValidation;

namespace Application.Services;

public class UsersService(IRepository<User> repository, IValidator<User> validator)
    : BaseService<User>(repository, validator)
{
    private readonly IRepository<User> _repository = repository;
    private readonly IValidator<User> _validator = validator;

    public async Task<User> Add(string name, string email, string password, CancellationToken ct)
    {
        var user = await new User(Guid.NewGuid(), name, email, password).ValidateAsync(_validator, ct);

        await _repository.Add(user, ct);
        return user;
    }

    public async Task<User> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await GetById(id, ct);
        user.Email = email ?? user.Email;
        user.PasswordHash = password ?? user.PasswordHash;

        await user.ValidateAsync(_validator, ct);
        return await _repository.Update(user, ct);
    }
}