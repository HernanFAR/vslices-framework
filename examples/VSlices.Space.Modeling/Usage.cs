using LanguageExt;
using VSlices.Space;

namespace VSlices.Space.Modeling;

/// <summary>
/// Compile-time modeling scenarios. These are usage probes, not automated tests.
/// </summary>
public static class Usage
{
    public static Fin<LocationName> CreateName(string value) =>
        Transformable.Transform<string, LocationName>(value);

    public static Fin<Location> Create() =>
        CreateName("Warehouse")
            .Bind(Create);

    public static Fin<Location> Create(LocationName name) =>
        Transformable.Transform<Location.Input, Location>(
            new Location.Input(name, 10, 20));

    /// <summary>
    /// X and Y are evolvable parts of Location.State.
    /// Name is intentionally absent: it is fixed by the first established state.
    /// </summary>
    public static Fin<Location> Move(Location location) =>
        location.Update(state => state with
        {
            X = state.X + 1,
            Y = state.Y + 1
        });

    /// <summary>
    /// Keeps both the source point and its creation-only name visible while proposing an evolution.
    /// The updated Location must preserve that same Name because State.Name cannot be changed by callers.
    /// </summary>
    public static (Location Original, LocationName Name, Fin<Location> Updated) PreserveSourceAndName(Location location) =>
        (
            location,
            location.CurrentState.Name,
            location.Update(state => state with { X = state.X + 10 })
        );
}
