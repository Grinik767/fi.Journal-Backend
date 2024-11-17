using System.Runtime.InteropServices.JavaScript;
using CSharpFunctionalExtensions;

namespace Domain.ValueTypes;

public class UserDiff : ValueObject
{
    public Guid TableId { get; }
    public Guid UserId { get; }
    public Dictionary<string, double> Diff;
    public DateTime UpdateTime { get; }

    private UserDiff(Guid tableId, Guid userId, Dictionary<string, double> diff, DateTime updateTime)
    {
        TableId = tableId;
        UserId = userId;
        Diff = diff;
        UpdateTime = updateTime;
    }

    public static Result<UserDiff, JSType.Error> Create(Guid tableId, Guid userId, Dictionary<string, double> diff, DateTime updateTime) 
        => new UserDiff(tableId, userId, diff, updateTime);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserId;
        yield return TableId;
        yield return UpdateTime;
    }
}