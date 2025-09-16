namespace Api.Contracts.User;

public record RegisterUserRequest(string Name, string Email, string Password, string StudyGroup);