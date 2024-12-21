using Api.Contracts.Table;
using Api.Dtos;
using Application.Services.Tables;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SuperAdmin")]
public class TablesController(ITablesService service, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateTableRequest request, CancellationToken ct)
    {
        var table = await service.Add(request.Name, request.Url, request.GroupId, request.HeaderRow, request.StudentColumn, ct, request.AdditionalData, request.ListToSearch);
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
    public async Task<List<FrontendTableDto>> GetAll(CancellationToken ct)
    {
        var tables = await service.GetAll(ct);
        return mapper.Map<List<FrontendTableDto>>(tables);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var table = await service.GetById(id, ct);
        return Ok(mapper.Map<FrontendTableDto>(table));
    }
    
    [HttpGet("{id:guid}/user/{userId:guid}")]
    public async Task<IActionResult> GetByIdWithCustomTable(
        Guid id,
        Guid userId, 
        CancellationToken ct)
    {
        var table = await service.GetTableWithCustomUrl(id, userId, ct);
        return Ok(mapper.Map<FrontendTableDto>(table));

    }
    
    [HttpGet("{id:guid}/userPoints/{userId:guid}")]
    public async Task<string> GetStudentPoints(
        Guid id, 
        Guid userId, 
        CancellationToken ct)
    {
        var points = await service.GetStudentPoint(userId, id, ct);
        return JsonConvert.SerializeObject(points);
    }
}