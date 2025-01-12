using Domain.Entities;

namespace Application.Services.Tables;

public interface ITablesService
{
    Task<Table> Add(string name, string url, Guid groupId, int headerRow, string studentColumn, CancellationToken ct,
        int additionalData, string listToSearch, string regulationsUrl);

    Task<Table> Update(Guid id, string? name, string? regulationsUrl, CancellationToken ct);
    Task Delete(Guid id, CancellationToken ct);
    Task<List<Table>> GetAll(CancellationToken ct);
    Task<Table> GetById(Guid id, CancellationToken ct);
    Task<Dictionary<string, double>> GetStudentPoint(Guid studentId, Guid tableId, CancellationToken ct);
    Task<Table> GetTableWithCustomUrl(Guid studentId, Guid tableId, CancellationToken ct);
}