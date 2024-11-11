using Domain.Entities;
using FluentValidation;

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
}