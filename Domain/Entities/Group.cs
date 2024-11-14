using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Group(Guid id, string name, Guid adminId) : Entity<Guid>(id)
{
    public readonly List<User> Users = [];
    public readonly List<Table> Tables = [];

    [Required] public string Name { get; set; } = name;
    [Required] public Guid AdminId { get; init; } = adminId;
    public User? Admin { get; init; }
}