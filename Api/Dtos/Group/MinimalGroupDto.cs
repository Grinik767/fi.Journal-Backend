namespace Api.Dtos.Group;

public class MinimalGroupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Guid[] TableIds { get; init; }
}