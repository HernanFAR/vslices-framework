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
            .Bind(name => Create(name));

    public static Fin<Location> Create(LocationName name) =>
        Transformable.Transform<Location.Input, Location>(
            new Location.Input(name, 10, 20));

    public static Fin<Location> Move(Location location) =>
        location.Update(state => state with
        {
            X = state.X + 1,
            Y = state.Y + 1
        });

    public static Fin<Location> Rename(Location location, string name) =>
        CreateName(name)
            .Bind(validName => Rename(location, validName));

    public static Fin<Location> Rename(Location location, LocationName name) =>
        location.Update(state => state with { Name = name });

    public static (Location Original, Fin<Location> Updated) PreserveSource(Location location) =>
        (location, location.Update(state => state with { X = state.X + 10 }));
}
