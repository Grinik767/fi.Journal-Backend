using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class EmailConfirmation(Guid id, Guid userId): Entity<Guid>(id)
{
    [Required] public Guid UserId { get; init; } = userId;
    public User? User { get; }
}