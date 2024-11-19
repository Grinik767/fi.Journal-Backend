using Api.Contracts.User;
using Api.Dtos;
using AutoMapper;
using Application.Services.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUsersService service, IMapper mapper, IOptions<AuthOptions> authOptions) : ControllerBase
{
    private readonly AuthOptions _authOptions = authOptions.Value;
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var user = await service.Register(request.Name, request.Email, request.Password, ct);
        return Ok(mapper.Map<UserDto>(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken ct)
    {
        var token = await service.Login(request.Email, request.Password, ct);
        
        HttpContext.Response.Cookies.Append(_authOptions.CookieName, token);
        
        return Ok(token);
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
}