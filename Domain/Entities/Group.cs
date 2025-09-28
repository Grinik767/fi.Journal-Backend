using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Group(Guid id, string name) : Entity<Guid>(id)
{
    private readonly List<Table> _tables = [];
    private readonly List<User> _users = [];
    [Required] public string Name { get; set; } = name;
    public IEnumerable<User> Users => _users;
    public IEnumerable<Table> Tables => _tables;
    public void AddUser(User user) => _users.Add(user);
    public void RemoveUser(User user) => _users.Remove(user);
}