namespace Api.Dtos;

public record UserDto(Guid Id, string Name, string Email, bool IsActive);