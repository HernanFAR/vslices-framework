namespace VSlices.Domain.Internals;

internal static class EntityAbstractions
{
    public static bool EntityEqualsTo(Entity @this, Entity? other)
    {
        if (other == null)
        {
            return false;
        }

        //Same instances must be considered as equal
        if (ReferenceEquals(@this, other))
        {
            return true;
        }

        //Must have a IS-A relation of types or must be same type
        var typeOfEntity1 = @this.GetType();
        var typeOfEntity2 = other.GetType();

        if (!typeOfEntity1.IsAssignableFrom(typeOfEntity2) && !typeOfEntity2.IsAssignableFrom(typeOfEntity1))
        {
            return false;
        }

        var entity1Keys = @this.GetKeys();
        var entity2Keys = other.GetKeys();

        for (var i = 0; i < entity1Keys.Length; i++)
        {
            var entity1Key = entity1Keys[i];
            var entity2Key = entity2Keys[i];

            if (!entity1Key.Equals(entity2Key))
            {
                return false;
            }
        }

        return true;
    }
}
