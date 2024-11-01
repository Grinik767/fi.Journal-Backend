namespace Api.Dtos;

public record TableDto(Guid Id, string Name, string Url, DateTime UpdateTime, Guid[] GroupIds);