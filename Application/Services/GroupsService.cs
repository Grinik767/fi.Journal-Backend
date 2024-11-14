using Application.Extensions;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;

namespace Application.Services;

public class GroupsService(
    GroupsRepository groupsRepository,
    UsersRepository usersRepository,
    IValidator<Group> validator)
{
    public async Task<Group?> Add(string name, Guid adminId, CancellationToken ct)
    {
        var admin = await usersRepository.GetById(adminId, ct);
        if (admin is null)
            return null;

        var group = await new Group(Guid.NewGuid(), name, adminId).Validate(validator, ct);
        if (group is not null)
            await groupsRepository.Add(group, ct);

        return group;
    }

    public async Task Delete(Guid id, CancellationToken ct) =>
        await groupsRepository.Delete(id, ct);

    public async Task<Group?> GetById(Guid id, CancellationToken ct) =>
        await groupsRepository.GetById(id, ct);

    public async Task<List<Group>> GetAll(CancellationToken ct) =>
        await groupsRepository.GetAll(ct);

    public async Task<Group?> Update(Guid id, string? name, CancellationToken ct) =>
        await groupsRepository.Update(id, name, ct);

    public async Task<List<User>?> GetUsers(Guid id, CancellationToken ct) =>
        await groupsRepository.GetUsers(id, ct);

    public async Task<Group?> AddUser(Guid id, Guid userId, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);
        if (group is null || group.AdminId == userId)
            return null;

        var user = await usersRepository.GetById(userId, ct);
        if (user is null || user.Groups.Contains(group))
            return null;

        group = await groupsRepository.AddOrDeleteUser(group, user, true, ct);
        return group;
    }

    public async Task<Group?> DeleteUser(Guid id, Guid userId, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);
        if (group is null)
            return null;

        var user = await usersRepository.GetById(userId, ct);
        if (user is null || !user.Groups.Contains(group))
            return null;

        group = await groupsRepository.AddOrDeleteUser(group, user, false, ct);
        return group;
    }
}