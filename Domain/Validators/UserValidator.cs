using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Name)
            .Length(3, 50);

        RuleFor(u => u.Email)
            .EmailAddress();

        RuleFor(u => u.PasswordHash)
            .NotEmpty();
    }
}