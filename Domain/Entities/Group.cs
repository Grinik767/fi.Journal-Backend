using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Group(string name, Guid adminId)
{
    public readonly List<User> Users = [];
    public readonly List<Table> Tables = [];
    private User? _admin;

    public Guid Id { get; init; }
    [Required] public string Name { get; set; } = name;

    [Required] public Guid AdminId { get; init; } = adminId;

    public User? Admin
    {
        get => _admin;
        set
        {
            if (value is null || value.Id != AdminId)
                throw new ValidationException();
            _admin = value;
        }
    }
}