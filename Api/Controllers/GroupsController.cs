using Api.Contracts;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController(JournalDbContext dbContext) : ControllerBase
{
    /*
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var group = new Group(request.Name);

        await dbContext.Groups.AddAsync(group, ct);
        await dbContext.SaveChangesAsync(ct);

        return Ok(group);
    }
    */

    [HttpGet]
    public async Task<List<Group>> GetAll(CancellationToken ct) =>
        await dbContext.Groups
            .AsNoTracking()
            .ToListAsync(ct);
}