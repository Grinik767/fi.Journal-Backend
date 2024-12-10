namespace Api.Contracts.Table;

public record CreateTableRequest(string Name, string Url, Guid GroupId, int HeaderRow, string StudentColumn,  int AdditionalData=-1, string ListToSearch = "");