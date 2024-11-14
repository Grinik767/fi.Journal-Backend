using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Group(Guid id, string name, Guid adminId) : Entity<Guid>(id)
{
    private readonly List<User> _users = [];
    private readonly List<Table> _tables = [];

    [Required] public string Name { get; set; } = name;
    [Required] public Guid AdminId { get; init; } = adminId;
    public User? Admin { get; init; }
    public IEnumerable<User> Users => _users;
    public IEnumerable<Table> Tables => _tables;
    
    public void AddUser(User user) => _users.Add(user);
    public void RemoveUser(User user) => _users.Remove(user);
}