namespace Domain.Entities;

public class Table(string name, string url, Guid adminId)
{
    public Guid Id { get; init; }
    public string Name { get; set; } = name;
    public string Url { get; init; } = url;

    public Guid AdminId { get; init; } = adminId;
    public User? Admin { get; init; }
    public List<Group> Groups { get; init; } = [];
}