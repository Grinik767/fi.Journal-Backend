using Api.Contracts;
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
        var admin = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.AdminId, ct);
        if (admin is null)
            return BadRequest();

        var table = new Table(request.Name, request.Url);
        await dbContext.Tables.AddAsync(table, ct);
        
        await dbContext.SaveChangesAsync(ct);
        return Ok();
    }
}