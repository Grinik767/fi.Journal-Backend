using Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.Entities;

[TestFixture]
public class TestsUser
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