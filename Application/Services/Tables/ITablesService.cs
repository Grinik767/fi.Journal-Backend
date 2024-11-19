using Domain.Entities;

namespace Application.Services.Tables;

public interface ITablesService
{
    Task<Table> Add(string name, string url, Guid groupId,  int headerRow, string studentColumn, CancellationToken ct, int additionalData=-1);
    Task<Table> Update(Guid id, string? name, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
    Task<List<Table>> GetAll(CancellationToken ct);
    Task<Table> GetById(Guid id, CancellationToken ct);
}