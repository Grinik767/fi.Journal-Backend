using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;

namespace Application.Services.Groups;

public class GroupsService(
    IRepository<Group> groupsRepository,
    IRepository<User> usersRepository,
    IValidator<Group> validator) : BaseService<Group>(groupsRepository, validator), IGroupsService
{
    private readonly IRepository<Group> _groupsRepository = groupsRepository;

    public async Task<Group> Add(string name, Guid adminId, CancellationToken ct)
    {
        var admin = await usersRepository.GetById(adminId, ct);
        return await base.Add(new Group(Guid.NewGuid(), name, admin.Id), ct);
    }

    public async Task<Group> Update(Guid id, string? name, CancellationToken ct)
    {
        var group = await _groupsRepository.GetById(id, ct);
        group.Name = name ?? group.Name;

        return await base.Update(group, ct);
    }

    public async Task<List<User>> GetUsers(Guid id, CancellationToken ct)
    {
        var group = await _groupsRepository.GetById(id, ct);
        return group.Users.ToList();
    }

    public async Task<List<Table>> GetTables(Guid id, CancellationToken ct)
    {
        var group = await _groupsRepository.GetById(id, ct);
        return group.Tables.ToList();
    }

    public async Task<Group> AddUser(Guid id, Guid userId, CancellationToken ct)
    {
        var group = await _groupsRepository.GetById(id, ct);
        if (group.AdminId == userId)
            throw new ArgumentException("User is an admin");

        var user = await usersRepository.GetById(userId, ct);
        if (user.Groups.Contains(group))
            throw new ArgumentException("User is already in group");

        group.AddUser(user);
        return await _groupsRepository.Update(group, ct);
    }

    public async Task<Group> DeleteUser(Guid id, Guid userId, CancellationToken ct)
    {
        var group = await _groupsRepository.GetById(id, ct);

        var user = await usersRepository.GetById(userId, ct);
        if (!user.Groups.Contains(group))
            throw new ArgumentException("User isn't in group");

        group.RemoveUser(user);
        return await _groupsRepository.Update(group, ct);
    }
}