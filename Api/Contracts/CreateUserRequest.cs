namespace Api.Contracts;

public record CreateUserRequest(string Name, string Email, string PasswordHash);