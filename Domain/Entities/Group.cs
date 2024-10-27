namespace Domain.Entities;

public class Group(string name)
{
    public Guid Id { get; init; }
    public string Name { get; init; } = name;
}