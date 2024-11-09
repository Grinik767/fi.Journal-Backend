namespace Api.Dtos;

public class TableDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Url { get; init; }
    public DateTime UpdateTime { get; init; }
    public GroupDto Group { get; init; }
}