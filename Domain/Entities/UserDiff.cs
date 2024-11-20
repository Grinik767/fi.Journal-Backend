using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserDiff(Guid id, Guid tableId, Guid userId, Dictionary<string, string> diff) : Entity<Guid>(id)
{
    public DateTime UpdateTime { get; } = DateTime.UtcNow;
    [Required] public Dictionary<string, string> Diff { get; } = diff;

    [Required] public Guid TableId { get; } = tableId;
    
    [Required] public Guid UserId { get; } = userId;

    public Table? Table { get; }
    public User? User { get; }
}