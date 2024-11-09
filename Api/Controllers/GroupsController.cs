using Api.Contracts.Group;
using Api.Dtos;
using Api.Extensions;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController(GroupsService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var group = await service.Add(request.Name, request.AdminId, ct);
        return await group.ToResult<GroupDto>(mapper, BadRequest);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task Delete(Guid id, CancellationToken ct) => await service.Delete(id, ct);

    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, string? name, CancellationToken ct)
    {
        var group = await service.Update(id, name, ct);
        return await group.ToResult<GroupDto>(mapper, BadRequest);
    }

    [HttpGet]
    public async Task<List<GroupDto>> GetAll(CancellationToken ct)
    {
        var groups = await service.GetAll(ct);
        return mapper.Map<List<GroupDto>>(groups);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var group = await service.GetById(id, ct);
        return await group.ToResult<GroupDto>(mapper, NotFound);
    }

    [HttpPost]
    [Route("{id:guid}/users")]
    public async Task<IActionResult> AddUser(Guid id, [FromBody] AddOrDeleteUserToGroupRequest request,
        CancellationToken ct)
    {
        var group = await service.AddUser(id, request.UserId, ct);
        return await group.ToResult<GroupDto>(mapper, BadRequest);
    }

    [HttpDelete]
    [Route("{id:guid}/users")]
    public async Task<IActionResult> DeleteUser(Guid id, [FromBody] AddOrDeleteUserToGroupRequest request,
        CancellationToken ct)
    {
        var group = await service.DeleteUser(id, request.UserId, ct);
        return await group.ToResult<GroupDto>(mapper, BadRequest);
    }
}