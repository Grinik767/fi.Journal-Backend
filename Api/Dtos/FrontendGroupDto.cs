namespace Api.Dtos;

public class FrontendGroupDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public Guid[] TableIds { get; init; }
}