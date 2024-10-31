using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Table(string name, string url, Guid adminId)
{
    public readonly List<Group> Groups = [];

    public Guid Id { get; init; }
    [Required] public string Name { get; set; } = name;
    [Required] public string Url { get; init; } = url;
    public DateTime UpdateTime { get; private set; } = DateTime.UtcNow;

    [Required] public Guid AdminId { get; init; } = adminId;
    public User? Admin { get; init; }
}