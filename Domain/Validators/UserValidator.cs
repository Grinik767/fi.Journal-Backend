using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty()
            .Length(3, 50);
        
        RuleFor(u => u.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(u => u.PasswordHash)
            .NotEmpty();
    }
}