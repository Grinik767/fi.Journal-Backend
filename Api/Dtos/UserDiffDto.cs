namespace Api.Dtos;

public class UserDiffDto
{
    public Guid Id { get; init; }
    public UserDto User { get; init; }
    public TableDto Table { get; init; }
    public DateTime UpdateTime { get; set; }
}