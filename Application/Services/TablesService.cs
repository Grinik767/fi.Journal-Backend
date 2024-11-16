using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;

namespace Application.Services;

public class TablesService(
    IRepository<Table> tablesRepository,
    IRepository<Group> groupsRepository,
    IValidator<Table> validator) : BaseService<Table>(tablesRepository, validator)
{
    private readonly IRepository<Table> _tablesRepository = tablesRepository;

    public async Task<Table> Add(string name, string url, Guid groupId, CancellationToken ct)
    {
        var group = await groupsRepository.GetById(groupId, ct);
        return await base.Add(new Table(Guid.NewGuid(), name, url, group.Id), ct);
    }

    public async Task<Table> Update(Guid id, string? name, CancellationToken ct)
    {
        var table = await _tablesRepository.GetById(id, ct);
        table.Name = name ?? table.Name;

        return await base.Update(table, ct);
    }
}