namespace Domain.Entities;

public class Entity
{
    public Guid Id { get; init; }

    protected bool Equals(Entity other) => EqualityComparer<Guid>.Default.Equals(Id, other.Id);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Entity)obj);
    }

    public override int GetHashCode() => EqualityComparer<Guid>.Default.GetHashCode(Id);
}