using Domain.Entities;
using NUnit.Framework;

namespace Tests.Domain.Entities;

[TestFixture]
public class TestsEntity
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