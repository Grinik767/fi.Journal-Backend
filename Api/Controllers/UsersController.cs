using Api.Contracts.User;
using Api.Dtos;
using AutoMapper;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var user = await service.Add(request.Name, request.Email, request.Password, ct);
        return await SendResult(user);
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task Delete(Guid id, CancellationToken ct) => await service.Delete(id, ct);

    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, string? email, string? password, CancellationToken ct)
    {
        var user = await service.Update(id, email, password, ct);
        return await SendResult(user);
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
        return await SendResult(user);
    }

    private Task<IActionResult> SendResult(User? user) =>
        Task.FromResult<IActionResult>(user is null ? NotFound() : Ok(mapper.Map<UserDto>(user)));
}