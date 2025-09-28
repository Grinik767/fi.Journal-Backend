using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories.Groups;
using Infrastructure.Repositories.UserDiffs;
using Infrastructure.Repositories.Users;

namespace Application.Services.Groups;

public class GroupsService(
    IGroupsRepository groupsRepository,
    IUsersRepository usersRepository,
    IUserDiffsRepository userDiffRepository,
    IValidator<Group> validator) : BaseService<Group>(groupsRepository, validator), IGroupsService
{
    public async Task<Group> Add(string name, CancellationToken ct)
    {
        return await base.Add(new Group(Guid.NewGuid(), name), ct);
    }

    public async Task<Group> Update(Guid id, string? name, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);
        group.Name = name ?? group.Name;

        return await base.Update(group, ct);
    }

    public async Task<Group> AddUser(Guid id, Guid userId, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);

        var user = await usersRepository.GetById(userId, ct);
        if (user.Groups.Contains(group))
            throw new ArgumentException("User is already in group");

        group.AddUser(user);
        return await groupsRepository.Update(group, ct);
    }

    public async Task<Group> DeleteUser(Guid id, Guid userId, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(id, ct);

        var user = await usersRepository.GetById(userId, ct);
        if (!user.Groups.Contains(group))
            throw new ArgumentException("User isn't in group");

        var diffs = await userDiffRepository.GetAll(ct);
        var userDiffs = diffs
            .Where(x => x.UserId == user.Id)
            .Where(x => group.Tables.Select(u => u.Id).Contains(x.TableId));

        foreach (var userDiff in userDiffs) 
            await userDiffRepository.Delete(userDiff.Id, ct);
        
        group.RemoveUser(user);
        return await groupsRepository.Update(group, ct);
    }
}