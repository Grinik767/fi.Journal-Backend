namespace Api.Dtos;

public class UserDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
    public bool IsEmailConfirmed { get; init; }
    
    public long TelegramId { get; init; }
    
    public Guid[] GroupIds { get; init; }
    public Guid[] GroupAsAdminIds{ get; init; }
}