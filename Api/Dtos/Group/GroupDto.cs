namespace Api.Dtos.Group;

public class GroupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Guid[] UserIds { get; init; }
    public Guid[] TableIds { get; init; }
}