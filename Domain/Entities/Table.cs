using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Table(string name, string url): Entity
{
    public readonly List<Group> Groups = [];
    [Required] public string Name { get; set; } = name;
    [Required] public string Url { get; init; } = url;
    
    public DateTime UpdateTime { get; private set; } = DateTime.UtcNow;
}