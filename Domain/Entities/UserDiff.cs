using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserDiff(Guid id, Guid tableId, Guid userId, Dictionary<string, string> diff) : Entity<Guid>(id)
{
    public DateTime UpdateTime { get; private set; } = DateTime.UtcNow;
    [Required] public Dictionary<string, string> Diff { get; init; } = diff;

    [Required] public Guid TableId { get; init; } = tableId;
    
    [Required] public Guid UserId { get; init; } = userId;

    public Table? Table { get; }
    public User? User { get; }
}