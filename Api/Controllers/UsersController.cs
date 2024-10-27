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

        return Ok(new UserDto(user.Id, user.Name, user.Email, user.IsActive));
    }

    [HttpGet]
    public async Task<List<UserDto>> GetAll(CancellationToken ct) =>
        await dbContext.Users
            .AsNoTracking()
            .Select(user => new UserDto(user.Id, user.Name, user.Email, user.IsActive))
            .ToListAsync(ct);

    [HttpGet]
    [Route("{guid:guid}")]
    public async Task<IActionResult> Get(Guid guid, CancellationToken ct)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == guid, cancellationToken: ct);

        return user is null
            ? NotFound()
            : Ok(new UserDto(user.Id, user.Name, user.Email, user.IsActive));
    }

    [HttpPatch]
    [Route("{guid:guid}")]
    public async Task<IActionResult> Update(Guid guid, string? email, string? passwordHash, bool? isActive,
        CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == guid, cancellationToken: ct);

        if (user is null)
            return NotFound();

        user.Update(email, passwordHash, isActive);
        await dbContext.SaveChangesAsync(ct);

        return Ok(new UserDto(user.Id, user.Name, user.Email, user.IsActive));
    }

    [HttpDelete]
    [Route("{guid:guid}")]
    public async Task Delete(Guid guid, CancellationToken ct) =>
        await dbContext.Users
            .Where(u => u.Id == guid)
            .ExecuteDeleteAsync(ct);
}