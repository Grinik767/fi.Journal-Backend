using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Tables;

public class TablesRepository(JournalDbContext dbContext) : ITablesRepository
{
    public async Task Add(Table table, CancellationToken ct)
    {
        await dbContext.Tables.AddAsync(table, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task Delete(Guid id, CancellationToken ct) =>
        await dbContext.Tables
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync(ct);

    public async Task<Table> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Tables
            .Include(t => t.Group)
            .ThenInclude(g => g!.Users)
            .FirstAsync(table => table.Id == id, ct);

    public async Task<List<Table>> GetAll(CancellationToken ct) =>
        await dbContext.Tables
            .AsNoTracking()
            .Include(t => t.Group)
            .ThenInclude(g => g.Users)
            .ToListAsync(ct);

    public async Task<Table> Update(Table table, CancellationToken ct)
    {
        await dbContext.SaveChangesAsync(ct);
        return table;
    }
}