namespace Domain.Entities;

public class TableGroup
{
    public Guid TableId { get; private set; }
    public Table Table { get; private set; }

    public Guid GroupId { get; private set; }
    public Group Group { get; private set; }

    public TableGroup(Guid tableId, Guid groupId)
    {
        TableId = tableId;
        GroupId = groupId;
    }
}