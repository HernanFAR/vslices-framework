using System.Runtime.CompilerServices;
using VSlices.Arrows;
using VSlices.Space.Traits;

namespace VSlices.Space.Modeling;

/// <summary>
/// Semantic location name. A valid instance is never empty and is at most 100 characters.
/// </summary>
public sealed record LocationName :
    DiscreteSpace<LocationName>,
    Transformable<string, LocationName>
{
    private LocationName(string value) =>
        Value = value;

    public string Value { get; }

    public static Req<string, LocationName>.Full Transformation =>
        Req<string, LocationName>.Ensure<string>(
            value => !string.IsNullOrWhiteSpace(value),
            "A location name cannot be empty.") >>
        Req<string, LocationName>.Ensure<string>(
            value => value.Trim().Length <= 100,
            "A location name cannot exceed 100 characters.") >>
        Req<string, LocationName>.Transform<string, LocationName>(
            value => new LocationName(value.Trim()));

    public override string ToString() =>
        Value;
}

/// <summary>
/// Modeling probe for creation and immutable evolution of a semantic point.
/// </summary>
public sealed class Location :
    Transformable<Location.Input, Location>,
    Evolvable<Location, Location.State>
{
    /// <summary>
    /// Information required to establish a Location. Name is already an established semantic value.
    /// </summary>
    public readonly record struct Input(LocationName Name, int X, int Y);

    public sealed record State
    {
        private State(LocationName name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public LocationName Name { get; }
        
        public int X { get; init; }
        
        public int Y { get; init; }
        
    }

    private Location(State state) =>
        CurrentState = state;

    public State CurrentState { get; }

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern State NewState(LocationName name, int x, int y);

    public static Req<Input, Location>.Full Transformation =>
        Req<Input, Location>.Transform<Input, Location>(
            input => new Location(NewState(input.Name, input.X, input.Y)));

    public static Req<State, Location>.Full Evolution =>
        Req<State, Location>.Transform<State, Location>(
            state => new Location(state));
}
