using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Group(string name, Guid adminId)
{
    public Guid Id { get; init; }
    [Required] public string Name { get; set; } = name;

    public Guid AdminId { get; init; } = adminId;
    public User? Admin { get; init; }

    public List<User> Users { get; init; } = [];
    public List<Table> Tables { get; init; } = [];
}