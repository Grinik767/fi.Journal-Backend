using Api.Contracts.Table;
using Api.Contracts.User;
using Api.Dtos;
using Api.Extensions;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController(TablesService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateTableRequest request, CancellationToken ct)
    {
        var table = await service.Add(request.Name, request.Url, request.GroupId, request.HeaderRow, request.StudentColumn, ct, request.AdditionalData);
        return await table.ToResult<TableDto>(mapper, BadRequest);
    }
    
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task Delete(Guid id, CancellationToken ct) => await service.Delete(id, ct);
    
    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, string? name, CancellationToken ct)
    {
        var table = await service.Update(id, name, ct);
        return await table.ToResult<TableDto>(mapper, BadRequest);
    }

    [HttpGet]
    public async Task<List<TableDto>> GetAll(CancellationToken ct)
    {
        var tables = await service.GetAll(ct);
        return mapper.Map<List<TableDto>>(tables);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var table = await service.GetById(id, ct);
        return await table.ToResult<TableDto>(mapper, NotFound);
    }
    
    [HttpPost("{id:guid}/users")]
    public async Task<string> GetStudentPoints(
        Guid id, 
        [FromBody] UserIdRequest user, 
        CancellationToken ct)
    {
        var points = await service.GetStudentPoint(user.userId, id, ct);
        return JsonConvert.SerializeObject(points);
    }
}