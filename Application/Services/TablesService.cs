using Domain.Entities;
using Infrastructure.Repositories;

namespace Application.Services;

public class TablesService(TablesRepository tablesRepository, GroupsRepository groupsRepository)
{
    public async Task<Table?> Add(string name, string url, Guid groupId,  int headerRow, string studentColumn, CancellationToken ct, int additionalData=-1)
    {
        var group = await groupsRepository.GetById(groupId, ct);
        if (group is null)
            return null;

        var table = new Table(Guid.NewGuid(), name, url, groupId, headerRow, studentColumn, additionalData);
        await tablesRepository.Add(table, ct);

        return table;
    }

    public async Task Delete(Guid id, CancellationToken ct) =>
        await tablesRepository.Delete(id, ct);

    public async Task<Table?> GetById(Guid id, CancellationToken ct) =>
        await tablesRepository.GetById(id, ct);

    public async Task<List<Table>> GetAll(CancellationToken ct) =>
        await tablesRepository.GetAll(ct);

    public async Task<Table?> Update(Guid id, string? name, CancellationToken ct) =>
        await tablesRepository.Update(id, name, ct);
}