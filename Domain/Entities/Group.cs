using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Group(Guid id, string name, Guid adminId) : Entity<Guid>(id)
{
    public readonly List<User> Users = [];
    public readonly List<Table> Tables = [];

    private string _name = name;

    [Required]
    public string Name
    {
        get => _name;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
                _name = value;
        }
    }

    [Required] public Guid AdminId { get; init; } = adminId;

    public User? Admin { get; init; }
}