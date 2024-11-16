using Api.Contracts.Table;
using Api.Dtos;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController(TablesService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateTableRequest request, CancellationToken ct)
    {
        var table = await service.Add(request.Name, request.Url, request.GroupId, ct);
        return Ok(mapper.Map<TableDto>(table)); ;
    }

    [HttpDelete("{id:guid}")]
    public async Task Delete(Guid id, CancellationToken ct) => await service.Delete(id, ct);

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, string? name, CancellationToken ct)
    {
        var table = await service.Update(id, name, ct);
        return Ok(mapper.Map<TableDto>(table));
    }

    [HttpGet]
    public async Task<List<TableDto>> GetAll(CancellationToken ct)
    {
        var tables = await service.GetAll(ct);
        return mapper.Map<List<TableDto>>(tables);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var table = await service.GetById(id, ct);
        return Ok(mapper.Map<TableDto>(table));
    }
}