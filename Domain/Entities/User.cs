using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User(string name, string email, string passwordHash)
{
    public Guid Id { get; init; }
    [Required] public string Name { get; init; } = name;
    [Required] public string Email { get; set; } = email;
    [Required] public string PasswordHash { get; set; } = passwordHash;
    public bool IsActive { get; set; }

    public List<Group> Groups { get; init; } = [];

    public List<Group> GroupsAsAdmin { get; init; } = [];
    public List<Table> TablesAsAdmin { get; init; } = [];
}