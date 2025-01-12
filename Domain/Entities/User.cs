using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User(Guid id, string name, string email, string passwordHash) : Entity<Guid>(id)
{
    private readonly List<Group> _groups = [];
    private readonly List<Group> _groupsAsAdmin = [];
    private readonly List<UserDiff> _userDiffs = [];
    private readonly List<UserNotification> _userNotifications = [];
    
    [Required] public string Name { get; init; } = name;
    [Required] public string Email { get; set; } = email;
    [Required] public string PasswordHash { get; set; } = passwordHash;
    public bool IsEmailConfirmed { get; set; }
    public long TelegramId { get; set; }
    
    public IEnumerable<Group> Groups => _groups;
    public IEnumerable<Group> GroupsAsAdmin => _groupsAsAdmin;
    public IEnumerable<UserDiff> UserDiffs => _userDiffs;
    public IEnumerable<UserNotification> UserNotifications => _userNotifications;
}