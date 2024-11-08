namespace Api.Contracts.User;

public record CreateUserRequest(string Name, string Email, string Password);