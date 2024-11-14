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
    public async Task<Group> Add(string name, Guid adminId, CancellationToken ct)
    {
        var admin = await usersRepository.GetById(adminId, ct);
        var group = await new Group(Guid.NewGuid(), name, admin.Id).ValidateAsync(validator, ct);
        
        await groupsRepository.Add(group, ct);
        return group;
    }

    public async Task Delete(Guid id, CancellationToken ct) => await groupsRepository.Delete(id, ct);

    public async Task<Group> GetById(Guid id, CancellationToken ct) => await groupsRepository.GetById(id, ct);

    public async Task<List<Group>> GetAll(CancellationToken ct) => await groupsRepository.GetAll(ct);

    public async Task<Group> Update(Guid id, string? name, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);
        group.Name = name ?? group.Name;
        
        await group.ValidateAsync(validator, ct);
        return await groupsRepository.Update(group, ct);
    }

    public async Task<List<User>> GetUsers(Guid id, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);
        return group.Users.ToList();
    }

    public async Task<Group> AddUser(Guid id, Guid userId, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);
        if (group.AdminId == userId)
            throw new ArgumentException("User is an admin");

        var user = await usersRepository.GetById(userId, ct);
        if (user.Groups.Contains(group))
            throw new ArgumentException("User is already in group");
        
        group.AddUser(user);
        return await groupsRepository.Update(group, ct);;
    }

    public async Task<Group> DeleteUser(Guid id, Guid userId, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);

        var user = await usersRepository.GetById(userId, ct);
        if (!user.Groups.Contains(group))
            throw new ArgumentException("User isn't in group");

        group.RemoveUser(user);
        return await groupsRepository.Update(group, ct);;
    }
}