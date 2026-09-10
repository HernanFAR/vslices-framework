#if MODELING_INVALID_USAGE
namespace VSlices.Space.Modeling;

public static class InvalidUsage
{
    public static Location.State CannotMintState()
    {
        // Expected compile failure: State construction is owned by Location.
        return new Location.State("Forged", 0, 0);
    }

    public static void CannotReplaceState(Location location)
    {
        // Expected compile failure: accepted state cannot be replaced from outside Location.
        location.State = location.State with { X = 999 };
    }
}
#endif
