namespace Api.Contracts.Table;

public record CreateTableRequest(string Name, string Url, Guid GroupId);