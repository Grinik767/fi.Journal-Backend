using Api.Contracts.User;
using Api.Dtos;
using AutoMapper;
using Application.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUsersService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var user = await service.Add(request.Name, request.Email, request.Password, ct);
        return Ok(mapper.Map<UserDto>(user));
    }

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id, CancellationToken ct) => await service.Delete(id, ct);

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await service.Update(id, email, password, ct);
        return Ok(mapper.Map<UserDto>(user));
    }

    [HttpGet]
    public async Task<List<UserDto>> GetAll(CancellationToken ct)
    {
        var users = await service.GetAll(ct);
        return mapper.Map<List<UserDto>>(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await service.GetById(id, ct);
        return Ok(mapper.Map<UserDto>(user));
    }
    
    [HttpGet("{id:guid}/groups")]
    public async Task<List<GroupDto>> GetGroups(Guid id, CancellationToken ct)
    {
        var groups = await service.GetGroups(id, ct);
        return mapper.Map<List<GroupDto>>(groups);
    }
    
    [HttpGet("{id:guid}/groupsAsAdmin")]
    public async Task<List<GroupDto>> GetGroupsAsAdmin(Guid id, CancellationToken ct)
    {
        var groupsAsAdmin = await service.GetGroupsAsAdmin(id, ct);
        return mapper.Map<List<GroupDto>>(groupsAsAdmin);
    }

    [HttpGet("{id:guid}/recentPoints")]
    public async Task<Dictionary<Guid, Dictionary<string, double>>> GetRecentUserPoints(Guid id, CancellationToken ct)
    {
        var points = await service.GetUserRecentPoints(id, ct);
        return points;
    }
}