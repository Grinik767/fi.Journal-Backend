namespace Api.Dtos.Table;

public class MinimalTableDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Url { get; init; }
    
    public string RegulationsUrl { get; init; }
}