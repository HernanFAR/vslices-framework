using VSlices.Domain.Internals;

namespace VSlices.Domain;

public abstract class Entity
{
    public override string ToString() => $"[{GetType().Name} | {WriteKeys()}]";

    public abstract object[] GetKeys();

    private string WriteKeys() => string.Join(", ", GetKeys());

    public bool EntityEquals(Entity? other) => EntityAbstractions.EntityEqualsTo(this, other);
}

public abstract class Entity<TKey> : Entity
{
    public TKey Id { get; private set; }

    protected Entity()
    {
        Id = default!;
    }

    protected Entity(TKey id)
    {
        Id = id;
    }

    public override object[] GetKeys() => new object[] { Id! };

}