using Domain.Entities;
using Infrastructure.DbContexts;

namespace Api.Services;

public class TableService(TableDbContext tableContext, GroupDbContext groupContext)
{
    public async Task<Table> CreateTableAsync(string name, string url, Guid groupId)
    {
        var group = await groupContext.Groups.FindAsync(groupId);
        if (group is null)
            throw new ArgumentException();

        var table = new Table(name, url, DateTime.UtcNow);
        tableContext.Tables.Add(table);

        var tableGroup = new TableGroup(table.Id, groupId);
        tableContext.TableGroups.Add(tableGroup);

        await tableContext.SaveChangesAsync();

        return table;
    }
}