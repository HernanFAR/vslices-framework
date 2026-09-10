#if MODELING_INVALID_USAGE
namespace VSlices.Space.Modeling;

public static class InvalidUsage
{
    public static Location.State CannotMintState(Location.Name name)
    {
        // Expected compile failure: State construction is owned by Location.
        return new Location.State(name, 0, 0);
    }

    public static Location.Name CannotMintName()
    {
        // Expected compile failure: Name must be established through its transformation rules.
        return new Location.Name("Forged");
    }

    public static void CannotReplaceState(Location location)
    {
        // Expected compile failure: accepted state cannot be replaced from outside Location.
        location.State = location.State with { X = 999 };
    }
}
#endif
