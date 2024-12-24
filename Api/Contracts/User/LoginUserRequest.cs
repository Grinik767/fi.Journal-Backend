namespace Api.Contracts.User;

public record LoginUserRequest(string Email, string Password, bool Remember);
