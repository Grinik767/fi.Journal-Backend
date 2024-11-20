using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Table(
    Guid id,
    string name,
    string url,
    Guid groupId,
    int headerRow,
    string studentColumn,
    int additionalData = -1) : Entity<Guid>(id)
{
    private readonly List<UserDiff> _userDiffs = [];
    [Required] public string Name { get; set; } = name;
    [Required] public string Url { get; init; } = url;
    [Required] public Guid GroupId { get; init; } = groupId;

    [Required] public int HeaderRow { get; init; } = headerRow;

    [Required] public string StudentColumn { get; init; } = studentColumn;

    public int AdditionalData { get; init; } = additionalData;
    public Group? Group { get; }
    public DateTime UpdateTime { get; set; } = DateTime.UtcNow;
    public IEnumerable<UserDiff> UserDiffs => _userDiffs;
}