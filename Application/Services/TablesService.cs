using Application.Extensions;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;

namespace Application.Services;

public class TablesService(
    TablesRepository tablesRepository,
    GroupsRepository groupsRepository,
    IValidator<Table> validator)
{
    public async Task<Table> Add(string name, string url, Guid groupId, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(groupId, ct);
        var table = await new Table(Guid.NewGuid(), name, url, group.Id).ValidateAsync(validator, ct);

        await tablesRepository.Add(table, ct);
        return table;
    }

    public async Task Delete(Guid id, CancellationToken ct) => await tablesRepository.Delete(id, ct);

    public async Task<Table> GetById(Guid id, CancellationToken ct) => await tablesRepository.GetById(id, ct);

    public async Task<List<Table>> GetAll(CancellationToken ct) => await tablesRepository.GetAll(ct);

    public async Task<Table> Update(Guid id, string? name, CancellationToken ct)
    {
        var table = await tablesRepository.GetById(id, ct);
        table.Name = name ?? table.Name;

        await table.ValidateAsync(validator, ct);
        return await tablesRepository.Update(table, ct);
    }
}