namespace Api.Dtos;

public class GroupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public UserDto Admin { get; init; }
    public Guid[] UserIds { get; init; }
}