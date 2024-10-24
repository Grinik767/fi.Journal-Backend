namespace Domain.Entities;


public class Group(string title)
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = title;
    public ICollection<TableGroup> TableGroups { get; private set; } = new List<TableGroup>();

    public void UpdateTitle(string title) => Title = title;
}