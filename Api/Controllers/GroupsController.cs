using Api.Contracts;
using Api.Dtos;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController(JournalDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var admin = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.GroupsAsAdmin)
            .FirstOrDefaultAsync(u => u.Id == request.AdminId, ct);
        if (admin is null || !admin.IsActive)
            return BadRequest();

        var group = new Group(request.Name, request.AdminId);
        await dbContext.Groups.AddAsync(group, ct);
        admin.GroupsAsAdmin.Add(group);

        group.Admin = admin;
        await dbContext.SaveChangesAsync(ct);
        return Ok(ToDto(group));
    }


    [HttpGet]
    public async Task<List<GroupDto>> GetAll(CancellationToken ct)
    {
        var groups = await dbContext.Groups
            .AsNoTracking()
            .Include(g => g.Admin)
            .ToListAsync(ct);

        return groups.Select(ToDto).ToList();
    }

    private static GroupDto ToDto(Group group) => new(
        group.Id,
        group.Name,
        UsersController.ToDto(group.Admin!),
        group.Users.Select(u => u.Id).ToArray()
    );
}