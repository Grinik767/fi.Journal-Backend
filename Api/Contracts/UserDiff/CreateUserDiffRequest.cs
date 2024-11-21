namespace Api.Contracts.UserDiff;

public record CreateUserDiffRequest(Guid userId, Guid tableId, Dictionary<string, double> diff);