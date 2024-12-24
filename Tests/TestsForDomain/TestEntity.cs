using NUnit.Framework;
using Domain.Entities;
using NUnit.Framework.Legacy;

namespace Domain.Tests;

[TestFixture]
public class TestEntity
{
    [Test]
    public void Entity_Equals_ReturnsTrueForSameId()
    {
        var entity1 = new Entity<Guid>(Guid.NewGuid());
        var entity2 = new Entity<Guid>(entity1.Id);
        Assert.That(entity1, Is.EqualTo(entity2));
    }

    [Test]
    public void Entity_Equals_ReturnsFalseForDifferentId()
    {
        var entity1 = new Entity<Guid>(Guid.NewGuid());
        var entity2 = new Entity<Guid>(Guid.NewGuid());
        Assert.That(entity1, !Is.EqualTo(entity2));
    }
}

[TestFixture]
public class GroupTests
{
    [Test]
    public void Group_Creation_WorksCorrectly()
    {
        var groupId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var group = new Group(groupId, "Test Group", adminId);

        Assert.That(groupId, Is.EqualTo(group.Id));
        Assert.That("Test Group", Is.EqualTo(group.Name));
        Assert.That(adminId, Is.EqualTo(group.AdminId));
        Assert.That(group.Users, Is.Empty);
        Assert.That(group.Tables, Is.Empty);
    }

    [Test]
    public void Group_AddUser_AddsUserToGroup()
    {
        var group = new Group(Guid.NewGuid(), "Test Group", Guid.NewGuid());
        var user = new User(Guid.NewGuid(), "Test User", "test@example.com", "passwordHash");

        group.AddUser(user);

        CollectionAssert.Contains(group.Users, user);
    }

    [Test]
    public void Group_RemoveUser_RemovesUserFromGroup()
    {
        var group = new Group(Guid.NewGuid(), "Test Group", Guid.NewGuid());
        var user = new User(Guid.NewGuid(), "Test User", "test@example.com", "passwordHash");

        group.AddUser(user);
        group.RemoveUser(user);

        CollectionAssert.DoesNotContain(group.Users, user);
    }
}

[TestFixture]
public class TableTests
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

[TestFixture]
public class UserTests
{
    [Test]
    public void User_Creation_WorksCorrectly()
    {
        var userId = Guid.NewGuid();
        var user = new User(userId, "Test User", "test@example.com", "passwordHash");

        Assert.That(userId, Is.EqualTo(user.Id));
        Assert.That("Test User", Is.EqualTo(user.Name));
        Assert.That("test@example.com", Is.EqualTo(user.Email));
        Assert.That("passwordHash", Is.EqualTo(user.PasswordHash));
        Assert.That(user.Groups, Is.Empty);
        Assert.That(user.GroupsAsAdmin, Is.Empty);
        Assert.That(user.UserDiffs, Is.Empty);
    }
}

[TestFixture]
public class UserDiffTests
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