using Domain.Entities;
using FluentValidation;

namespace Application.Extensions;

internal static class EntityExtensions
{
    public static async Task ValidateAsync<T>(this T entity, IValidator<T> validator, CancellationToken ct)
        where T : Entity<Guid>
    {
        var validationResult = await validator.ValidateAsync(entity, ct);
        if (!validationResult.IsValid)
            throw new ArgumentException("Arguments aren't valid");
    }
}