using Domain.Entities;
using FluentValidation;


namespace Domain.Validators;

public class TableValidator: AbstractValidator<Table>
{
    public TableValidator()
    {
        RuleFor(t => t.Name)
            .Length(3, 20);
        
        RuleFor(t => t.Url)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .Must(url => url.StartsWith("https://docs.google.com/spreadsheets/"));

        RuleFor(t => t.UpdateTime)
            .Must(dt => dt.Kind == DateTimeKind.Utc);
    }
}