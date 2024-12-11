using Api.Contracts.Group;
using Api.Dtos;
using Application.Services.Groups;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController(IGroupsService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var group = await service.Add(request.Name, request.AdminId, ct);
        return Ok(mapper.Map<GroupDto>(group));
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task Delete(Guid id, CancellationToken ct) => await service.Delete(id, ct);

    [HttpPatch("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, string? name, CancellationToken ct)
    {
        var group = await service.Update(id, name, ct);
        return Ok(mapper.Map<GroupDto>(group));
    }

    [HttpGet]
    [Authorize]
    public async Task<List<FrontendGroupDto>> GetAll(CancellationToken ct)
    {
        var groups = await service.GetAll(ct);
        return mapper.Map<List<FrontendGroupDto>>(groups);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var group = await service.GetById(id, ct);
        return Ok(mapper.Map<FrontendGroupDto>(group));
    }

    [HttpGet("{id:guid}/users")]
    [Authorize]
    public async Task<List<UserDto>> GetUsers(Guid id, CancellationToken ct)
    {
        var users = await service.GetUsers(id, ct);
        return mapper.Map<List<UserDto>>(users);
    }
    
    [HttpGet("{id:guid}/tables")]
    [Authorize]
    public async Task<List<FrontendTableDto>> GetTables(Guid id, CancellationToken ct)
    {
        var tables = await service.GetTables(id, ct);
        return mapper.Map<List<FrontendTableDto>>(tables);
    }

    [HttpPost("{id:guid}/users")]
    [Authorize]
    public async Task<IActionResult> AddUser(Guid id, [FromBody] AddOrDeleteUserToGroupRequest request,
        CancellationToken ct)
    {
        var group = await service.AddUser(id, request.UserId, ct);
        return Ok(mapper.Map<GroupDto>(group));
    }

    [HttpDelete("{id:guid}/users")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(Guid id, [FromBody] AddOrDeleteUserToGroupRequest request,
        CancellationToken ct)
    {
        var group = await service.DeleteUser(id, request.UserId, ct);
        return Ok(mapper.Map<GroupDto>(group));
    }
}