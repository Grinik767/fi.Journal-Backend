using Api.Contracts.Table;
using Api.Contracts.UserDiff;
using Api.Dtos;
using Application.Services.Tables;
using Application.Services.UserDiffs;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserDiffController(IUserDiffsService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserDiffRequest request, CancellationToken ct)
    {
        var userDiff = await service.Add(request.diff, request.userId, request.tableId, ct);
        return Ok(mapper.Map<UserDiffDto>(userDiff));
    }
    
    [HttpGet]
    public async Task<List<UserDiffDto>> GetAll(CancellationToken ct)
    {
        var userDiffs = await service.GetAll(ct);
        return mapper.Map<List<UserDiffDto>>(userDiffs);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var userDiff = await service.GetById(id, ct);
        return Ok(mapper.Map<UserDiffDto>(userDiff));
    }

    [HttpGet("/user/{userId:guid}")]
    public async Task<List<Tuple<DateTime, Guid, Dictionary<string, double>>>> GetUsersDiff(Guid userId, CancellationToken ct)
    {
        var result = await service.GetDiffForUser(userId, ct);
        return result.Select(x => x.ToTuple()).ToList();
    }
}