using Api.Contracts;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly JournalDbContext _dbContext;

    public GroupsController(JournalDbContext dbContext) => _dbContext = dbContext;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var group = new Group(request.Name);

        await _dbContext.Groups.AddAsync(group, ct);
        await _dbContext.SaveChangesAsync(ct);

        return Ok(group);
    }
}