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
        if (admin is null)
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
            .Include(g => g.Users)
            .ToListAsync(ct);

        return groups.Select(ToDto).ToList();
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var group = await GetGroupById(id, ct);
        return group is null
            ? NotFound()
            : Ok(ToDto(group));
    }


    [HttpPost]
    [Route("{id:guid}/users")]
    public async Task<IActionResult> AddUser(Guid id, [FromBody] AddOrDeleteUserToGroupRequest request,
        CancellationToken ct)
    {
        var group = await GetGroupById(id, ct);
        if (group is null || request.UserId == group.AdminId)
            return BadRequest();

        var user = await dbContext.Users
            .Include(u => u.Groups)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
        if (user is null || user.Groups.Contains(group))
            return BadRequest();

        group.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        return Ok(ToDto(group));
    }

    [HttpDelete]
    [Route("{id:guid}/users")]
    public async Task<IActionResult> DeleteUser(Guid id, [FromBody] AddOrDeleteUserToGroupRequest request,
        CancellationToken ct)
    {
        var group = await GetGroupById(id, ct);
        if (group is null)
            return BadRequest();
        
        var user = await dbContext.Users
            .Include(u => u.Groups)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
        if (user is null || !user.Groups.Contains(group))
            return BadRequest();
        
        group.Users.Remove(user);
        await dbContext.SaveChangesAsync(ct);
        
        return Ok(ToDto(group));
    }

    private static GroupDto ToDto(Group group) => new(
        group.Id,
        group.Name,
        UsersController.ToDto(group.Admin!),
        group.Users.Select(u => u.Id).ToArray()
    );

    private async Task<Group?> GetGroupById(Guid id, CancellationToken ct) => await dbContext.Groups
        .Include(g => g.Admin)
        .Include(g => g.Users)
        .FirstOrDefaultAsync(g => g.Id == id, ct);
}