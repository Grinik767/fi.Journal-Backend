using Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.Entities;

[TestFixture]
public class TestsTable
{
    [Test]
    public void Table_Creation_WorksCorrectly()
    {
        var tableId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        var table = new Table(tableId, "Test Table", "http://example.com", groupId, 1, "Student Column");

        Assert.That(tableId, Is.EqualTo(table.Id));
        Assert.That("Test Table", Is.EqualTo(table.Name));
        Assert.That("http://example.com", Is.EqualTo(table.Url));
        Assert.That(groupId, Is.EqualTo(table.GroupId));
        Assert.That(1, Is.EqualTo(table.HeaderRow));
        Assert.That("Student Column", Is.EqualTo(table.StudentColumn));
        Assert.That(table.UserDiffs, Is.Empty);
    }
}