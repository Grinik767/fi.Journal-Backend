namespace Api.Dtos;

public record GroupDto(Guid Id, string Name, UserDto Admin, Guid[] UserIds);