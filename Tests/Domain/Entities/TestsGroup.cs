using Domain.Entities;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Tests.Domain.Entities;

[TestFixture]
public class TestsGroup
{
    [Test]
    public void Group_Creation_WorksCorrectly()
    {
        var groupId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group");

        Assert.That(groupId, Is.EqualTo(group.Id));
        Assert.That("Test Group", Is.EqualTo(group.Name));
        Assert.That(group.Users, Is.Empty);
        Assert.That(group.Tables, Is.Empty);
    }

    [Test]
    public void Group_AddUser_AddsUserToGroup()
    {
        var group = new Group(Guid.NewGuid(), "Test Group");
        var user = new User(Guid.NewGuid(), "Test User", "test@example.com", "passwordHash", "ФТ-102-2");

        group.AddUser(user);

        CollectionAssert.Contains(group.Users, user);
    }

    [Test]
    public void Group_RemoveUser_RemovesUserFromGroup()
    {
        var group = new Group(Guid.NewGuid(), "Test Group");
        var user = new User(Guid.NewGuid(), "Test User", "test@example.com", "passwordHash", "ФТ-102-2");

        group.AddUser(user);
        group.RemoveUser(user);

        CollectionAssert.DoesNotContain(group.Users, user);
    }
}