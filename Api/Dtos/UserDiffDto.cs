namespace Api.Dtos;

public class UserDiffDto
{
    public Guid Id { get; init; }
    public TableDto Table { get; init; }
    public DateTime UpdateTime { get; init; }
    public required Dictionary<string, double> Diff { get; init; }
}