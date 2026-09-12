#if MODELING_INVALID_USAGE
namespace VSlices.Space.Modeling;

public static class InvalidUsage
{
    public static Location.State CannotMintState(LocationName name)
    {
        // Expected compile failure: State construction is owned by Location.
        return new Location.State(name, 0, 0);
    }

    public static LocationName CannotMintName()
    {
        // Expected compile failure: Name must be established through its transformation rules.
        return new LocationName("Forged");
    }

    public static Location.State CannotChangeCreationOnlyName(Location location, LocationName anotherName)
    {
        // Expected compile failure: Name is get-only once the first Location.State is established.
        return location.CurrentState with { Name = anotherName };
    }

    public static void CannotReplaceAcceptedState(Location location)
    {
        // Expected compile failure: the currently accepted State is get-only on Location.
        location.CurrentState = location.CurrentState with { X = 999 };
    }
}
#endif
