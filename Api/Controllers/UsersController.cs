using Api.Contracts.User;
using Api.Dtos;
using Api.Extensions;
using AutoMapper;
using Application.Services;
using Domain.ValueTypes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UsersService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var user = await service.Add(request.Name, request.Email, request.Password, ct);
        return await user.ToResult<UserDto>(mapper, BadRequest);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task Delete(Guid id, CancellationToken ct) => await service.Delete(id, ct);

    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await service.Update(id, email, password, ct);
        return await user.ToResult<UserDto>(mapper, BadRequest);
    }

    [HttpGet]
    public async Task<List<UserDto>> GetAll(CancellationToken ct)
    {
        var users = await service.GetAll(ct);
        return mapper.Map<List<UserDto>>(users);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var user = await service.GetById(id, ct);
        return await user.ToResult<UserDto>(mapper, NotFound);
    }

    [HttpGet]
    [Route("{id:guid}/recent")]
    public async Task<Dictionary<Guid, Dictionary<string, double>>> GetRecentPoint(Guid id, CancellationToken ct)
    {
        var studentPoints = await service.GetUserRecentPoints(id, ct);
        return studentPoints;
    }
    
    [HttpGet]
    [Route("{id:guid}/diff")]
    public async Task<List<UserDiff>> GetUserDiff(Guid id, CancellationToken ct)
    {
        var studentPoints = await service.GetStudentDiff(id, ct);
        return studentPoints;
    }
}