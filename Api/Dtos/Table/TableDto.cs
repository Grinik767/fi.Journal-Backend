using Api.Dtos.Group;

namespace Api.Dtos;

public class TableDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Url { get; init; }
    
    public string StudentColumn { get; init; }
    
    public int HeaderRow { get; init; }
    
    public int AdditionalData { get; init; }
    
    public DateTime UpdateTime { get; init; }
    public string ListToSearch { get; init; }
    
    public string RegulationsUrl { get; init; }
    public GroupDto Group { get; init; }
}