using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;

namespace Application.Services.Users;

public class UsersService(IRepository<User> repository, IValidator<User> validator)
    : BaseService<User>(repository, validator), IUsersService
{
    private readonly IRepository<User> _repository = repository;

    public async Task<User> Add(string name, string email, string password, CancellationToken ct) =>
        await base.Add(new User(Guid.NewGuid(), name, email, password), ct);

    public async Task<User> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await _repository.GetById(id, ct);
        user.Email = email ?? user.Email;
        user.PasswordHash = password ?? user.PasswordHash;

        return await base.Update(user, ct);
    }

    public async Task<List<Group>> GetGroups(Guid id, CancellationToken ct)
    {
        var user = await _repository.GetById(id, ct);
        return user.Groups.ToList();
    }

    public async Task<List<Group>> GetGroupsAsAdmin(Guid id, CancellationToken ct)
    {
        var user = await _repository.GetById(id, ct);
        return user.GroupsAsAdmin.ToList();
    }
}