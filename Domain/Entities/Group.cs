namespace Domain.Entities;

public class Group
{
    public Group(string name)
    {
        Name = name;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
}