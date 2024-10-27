namespace Domain.Entities;

public class User(string name, string email, string passwordHash)
{
    public Guid Id { get; init; }
    public string Name { get; init; } = name;
    public string Email { get; set; } = email;
    public string PasswordHash { get; set; } = passwordHash;
    public bool IsActive { get; set; } = false;
    
    public List<Group> Groups { get; init; } = [];

    public List<Group> GroupsAsAdmin { get; init; } = [];
    public List<Table> TablesAsAdmin { get; init; } = []; 
}