namespace Api.Contracts;

public record CreateTableRequest(string Name, string Url, Guid[] GroupIds);