using Api.Contracts;
using Api.Dtos;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;
/*
[ApiController]
[Route("api/[controller]")]
public class TablesController(JournalDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTableRequest request, CancellationToken ct)
    {
        var groups = await dbContext.Groups
            .Include(g => g.Tables)
            .Where(g => request.GroupIds.Contains(g.Id))
            .ToArrayAsync(ct);

        if (groups.Length != request.GroupIds.Length)
            return BadRequest();

        var table = new Table(request.Name, request.Url);
        await dbContext.Tables.AddAsync(table, ct);

        foreach (var group in groups)
            group.Tables.Add(table);

        await dbContext.SaveChangesAsync(ct);
        return Ok(ToDto(table));
    }

    [HttpGet]
    public async Task<List<TableDto>> GetAll(CancellationToken ct)
    {
        var tables = await dbContext.Tables
            .AsNoTracking()
            .Include(t => t.Groups)
            .ToListAsync(ct);

        return tables.Select(ToDto).ToList();
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var table = await GetTableById(id, ct);
        return table is null
            ? NotFound()
            : Ok(ToDto(table));
    }

    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupOrTableRequest request, CancellationToken ct)
    {
        var table = await GetTableById(id, ct);
        if (table is null)
            return BadRequest();

        table.Name = request.Name ?? table.Name;

        await dbContext.SaveChangesAsync(ct);
        return Ok(ToDto(table));
    }
    
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task Delete(Guid id, CancellationToken ct) =>
        await dbContext.Tables
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(ct);

    private static TableDto ToDto(Table table) =>
        new(
            table.Id,
            table.Name,
            table.Url,
            table.UpdateTime,
            table.Groups.Select(g => g.Id).ToArray()
        );

    private async Task<Table?> GetTableById(Guid id, CancellationToken ct) => await dbContext.Tables
        .Include(t => t.Groups)
        .FirstOrDefaultAsync(t => t.Id == id, ct);
}
*/