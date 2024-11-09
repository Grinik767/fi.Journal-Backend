using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TablesRepository(JournalDbContext dbContext) : IRepository<Table>
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

    public async Task<Table?> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Tables
            .Include(t => t.Group)
            .ThenInclude(g => g!.Admin)
            .FirstOrDefaultAsync(table => table.Id == id, ct);

    public async Task<List<Table>> GetAll(CancellationToken ct) =>
        await dbContext.Tables
            .AsNoTracking()
            .Include(t => t.Group)
            .ThenInclude(g => g!.Admin)
            .ToListAsync(ct);

    public async Task<Table?> Update(Guid id, string? name, CancellationToken ct)
    {
        var table = await GetById(id, ct);
        if (table is null)
            return table;

        table.Name = name ?? table.Name;
        
        await dbContext.SaveChangesAsync(ct);
        return table;
    }
}