namespace Api.Dtos;

public class UserDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    
    public Guid[] Groups { get; set; }
    
    public Guid[] GroupsAsAdminIds { get; set; }
}