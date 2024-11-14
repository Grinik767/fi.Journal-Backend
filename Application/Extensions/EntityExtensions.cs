using Domain.Entities;
using FluentValidation;
using Infrastructure.Repositories;

namespace Application.Extensions;

internal static class EntityExtensions
{
    public static async Task<T?> Validate<T>(this T? entity, IValidator<T> validator, CancellationToken ct)
        where T : Entity<Guid>
    {
        if (entity is null)
            return null;

        var validationResult = await validator.ValidateAsync(entity, ct);
        return validationResult.IsValid ? entity : null;
    }

    public static async Task<TEntity?> EnsureExist<TEntity, TRepository>(this TEntity entity, TRepository repository,
        CancellationToken ct)
        where TEntity : Entity<Guid>
        where TRepository : IRepository<TEntity>
    {
        var result = await repository.GetById(entity.Id, ct);
        if (result is null)
            throw new ArgumentException("Entity not found");

        return result;
    }
}