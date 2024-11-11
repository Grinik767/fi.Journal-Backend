using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User(Guid id, string name, string email, string passwordHash) : Entity<Guid>(id)
{
    public readonly List<Group> Groups = [];
    public readonly List<Group> GroupsAsAdmin = [];

    [Required] public string Name { get; init; } = name;

    [Required] public string Email { get; set; } = email;

    [Required] public string PasswordHash { get; set; } = passwordHash;
}