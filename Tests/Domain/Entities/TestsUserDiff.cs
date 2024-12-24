using Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.Entities;

[TestFixture]
public class TestsUserDiff
{
    [Test]
    public void UserDiff_Creation_WorksCorrectly()
    {
        var diffId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var diff = new Dictionary<string, string> { { "Key", "Value" } };

        var userDiff = new UserDiff(diffId, tableId, userId, diff);

        Assert.That(diffId, Is.EqualTo(userDiff.Id));
        Assert.That(tableId, Is.EqualTo(userDiff.TableId));
        Assert.That(userId, Is.EqualTo(userDiff.UserId));
        Assert.That(diff, Is.EqualTo(userDiff.Diff));
    }

    [Test]
    public void UserDiff_UpdateTime_IsSetToUtcNow()
    {
        var diffId = Guid.NewGuid();
        var tableId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var diff = new Dictionary<string, string>();

        var userDiff = new UserDiff(diffId, tableId, userId, diff);

        Assert.That(userDiff.UpdateTime, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
    }
}