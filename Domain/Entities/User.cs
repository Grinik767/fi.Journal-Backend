using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User(string name, string email, string passwordHash) : Entity
{
    public readonly List<Group> Groups = [];
    public readonly List<Group> GroupsAsAdmin = [];
    public readonly List<Table> TablesAsAdmin = [];

    [Required] public string Name { get; init; } = name;
    [Required] public string Email { get; set; } = email;
    [Required] public string PasswordHash { get; set; } = passwordHash;
    public bool IsActive { get; set; }

    public void Update(string? email, string? passwordHash, bool? isActive)
    {
        Email = email ?? Email;
        PasswordHash = passwordHash ?? PasswordHash;
        IsActive = isActive ?? IsActive;
    }
}