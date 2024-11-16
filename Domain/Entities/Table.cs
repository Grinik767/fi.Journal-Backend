using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Table(Guid id, string name, string url, Guid groupId) : Entity<Guid>(id)
{
    [Required] public string Name { get; set; } = name;
    [Required] public string Url { get; init; } = url;
    [Required] public Guid GroupId { get; init; } = groupId;
    public Group? Group { get; }
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
}