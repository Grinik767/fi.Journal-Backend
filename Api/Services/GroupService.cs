using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Api.Services;

public class GroupService(GroupDbContext groupContext)
{
    public async Task<Group> CreateGroupAsync(string name)
    {
        var group = new Group(name);
        groupContext.Groups.Add(group);
        await groupContext.SaveChangesAsync();
        return group;
    }

    public async Task<IList<Group>> GetAllGroupsAsync() => await groupContext.Groups.ToListAsync();

    public async Task<IList<Table>> GetTablesByGroup(Guid id)
    {
        return await groupContext.TableGroups
            .Where(tg => tg.GroupId == id)
            .Select(tg => tg.Table)
            .ToListAsync();
    }
}