namespace Domain.Entities;

public class Entity<TId>(TId id)
{
    public TId Id { get; } = id;

    protected bool Equals(Entity<TId> other) => EqualityComparer<TId>.Default.Equals(Id, other.Id);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Entity<TId>)obj);
    }

    public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id);
}