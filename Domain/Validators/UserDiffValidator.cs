using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class UserDiffValidator : AbstractValidator<UserDiff>
{
    public UserDiffValidator()
    {
        RuleFor(d => d.UpdateTime)
            .Must(dt => dt.Kind == DateTimeKind.Utc);
    }
}