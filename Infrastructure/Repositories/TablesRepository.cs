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
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);

    public async Task<Table?> GetById(Guid id, CancellationToken ct) =>
        await dbContext.Tables
            .Include(t => t.Group)
            .FirstOrDefaultAsync(table => table.Id == id, ct);

    public async Task<List<Table>> GetAll(CancellationToken ct) =>
        await dbContext.Tables
            .AsNoTracking()
            .Include(t => t.Group)
            .ToListAsync(ct);
}