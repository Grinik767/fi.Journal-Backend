using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class GroupValidator : AbstractValidator<Group>
{
    public GroupValidator()
    {
        RuleFor(g => g.Name)
            .Length(3, 30);
    }
}