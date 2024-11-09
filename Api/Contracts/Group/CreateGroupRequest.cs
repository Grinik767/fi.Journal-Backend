namespace Api.Contracts.Group;

public record CreateGroupRequest(string Name, Guid AdminId);