namespace Domain.Entities;

public class Table(string title, string url, DateTime lastUpdate)
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = title;
    public string Url { get; private set; } = url;
    public DateTime LastUpdate { get; private set; } = lastUpdate;
    public ICollection<TableGroup> TableGroups { get; private set; } = new List<TableGroup>();

    public void UpdateTitle(string title) => Title = title;
}