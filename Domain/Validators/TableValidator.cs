using Domain.Entities;
using FluentValidation;


namespace Domain.Validators;

public class TableValidator: AbstractValidator<Table>
{
    public TableValidator()
    {
        RuleFor(t => t.Name)
            .Length(3, 30);
        RuleFor(t => t.Url)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute));
    }
}