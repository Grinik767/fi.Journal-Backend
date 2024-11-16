using Domain.Entities;
using FluentValidation;

namespace Application.Extensions;

internal static class EntityExtensions
{
    public static async Task<T> ValidateAsync<T>(this T entity, IValidator<T> validator, CancellationToken ct)
        where T : Entity<Guid>
    {
        var validationResult = await validator.ValidateAsync(entity, ct);
        return validationResult.IsValid ? entity : throw new ArgumentException("Arguments aren't valid");
    }
}