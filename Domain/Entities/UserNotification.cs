using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class UserNotification : Entity<Guid>
{
    public Guid Id { get; private set; }

    [Required]
    public Guid UserId { get; private set; }

    [Required]
    public Guid UserDiffId { get; private set; }

    public User? User { get; }
    public UserDiff? UserDiff { get; }
    
    public UserNotification(Guid id, Guid userId, Guid userDiffId) : base(id)
    {
        Id = id;
        UserId = userId;
        UserDiffId = userDiffId;
    }
    
    private UserNotification() : base(default) { }
}