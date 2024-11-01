using Api.Contracts;
using Api.Dtos;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

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

    private static TableDto ToDto(Table table) =>
        new(
            table.Id,
            table.Name,
            table.Url,
            table.UpdateTime,
            table.Groups.Select(g => g.Id).ToArray()
        );
}