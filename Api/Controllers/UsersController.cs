using Api.Contracts;
using Api.Dtos;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(JournalDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var user = new User(request.Name, request.Email, request.PasswordHash);

        await dbContext.Users.AddAsync(user, ct);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new UserDto(user.Id, user.Name, user.Email, user.IsActive, [], []));
    }

    [HttpGet]
    public async Task<List<UserDto>> GetAll(CancellationToken ct) =>
        await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Groups)
            .Include(u => u.GroupsAsAdmin)
            .Select(user => ToDto(user))
            .ToListAsync(ct);

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Groups)
            .Include(u => u.GroupsAsAdmin)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken: ct);

        return user is null
            ? NotFound()
            : Ok(ToDto(user));
    }

    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, string? email, string? passwordHash, bool? isActive,
        CancellationToken ct)
    {
        var user = await dbContext.Users
            .Include(u => u.Groups)
            .Include(u => u.GroupsAsAdmin)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken: ct);

        if (user is null)
            return NotFound();

        user.Update(email, passwordHash, isActive);
        await dbContext.SaveChangesAsync(ct);

        return Ok(ToDto(user));
    }

    [HttpDelete]
    [Route("{guid:guid}")]
    public async Task Delete(Guid guid, CancellationToken ct) =>
        await dbContext.Users
            .Where(u => u.Id == guid)
            .ExecuteDeleteAsync(ct);

    public static UserDto ToDto(User user) => new(user.Id, user.Name, user.Email, user.IsActive,
        user.Groups.Select(g => g.Id).ToArray(),
        user.GroupsAsAdmin.Select(g => g.Id).ToArray());
}