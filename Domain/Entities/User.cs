using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User(Guid id, string name, string email, string passwordHash) : Entity<Guid>(id)
{
    public readonly List<Group> Groups = [];
    public readonly List<Group> GroupsAsAdmin = [];
    
    private string _email = email;
    private string _passwordHash = passwordHash;

    [Required] public string Name { get; init; } = name;

    [Required]
    public string Email
    {
        get => _email;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                _email = value;
        }
    }

    [Required]
    public string PasswordHash
    {
        get => _passwordHash;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                _passwordHash = value;
        }
    }
}