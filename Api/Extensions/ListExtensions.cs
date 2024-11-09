using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ListExtensions
{
    public static Task<IActionResult> ToResult<TIn, TDto>(this List<TIn>? entities, IMapper mapper,
        Func<IActionResult> errorResult) where TIn : Entity<Guid> =>
        Task.FromResult(entities is null ? errorResult() : new OkObjectResult(mapper.Map<TDto>(entities)));
}