using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class EntityExtensions
{
    public static Task<IActionResult> ToResult<TDto>(this Entity<Guid>? entity, IMapper mapper,
        Func<IActionResult> errorResult) =>
        Task.FromResult(entity is null ? errorResult() : new OkObjectResult(mapper.Map<TDto>(entity)));
}