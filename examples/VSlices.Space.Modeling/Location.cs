using System.Runtime.CompilerServices;
using VSlices.Arrows;
using VSlices.Space;

namespace VSlices.Space.Modeling;

/// <summary>
/// Modeling probe for creation and immutable evolution of a semantic point.
/// </summary>
public sealed class Location :
    Transformable<Location.Input, Location>,
    Evolvable<Location, Location.State>
{
    /// <summary>
    /// Semantic location name. A valid instance is never empty and is at most 100 characters.
    /// </summary>
    public sealed record Name :
        DiscreteSpace<Name>,
        Transformable<string, Name>
    {
        private Name(string value) =>
            Value = value;

        public string Value { get; }

        public static Req<string, Name>.Full Transformation =>
            Req<string, Name>.Ensure<string>(
                value => !string.IsNullOrWhiteSpace(value),
                "A location name cannot be empty.") >>
            Req<string, Name>.Ensure<string>(
                value => value.Trim().Length <= 100,
                "A location name cannot exceed 100 characters.") >>
            Req<string, Name>.Transform<string, Name>(
                value => new Name(value.Trim()));

        public override string ToString() =>
            Value;
    }

    /// <summary>
    /// Information required to establish a Location. Name is already an established semantic value.
    /// </summary>
    public readonly record struct Input(Name Name, int X, int Y);

    public sealed record State
    {
        private State(Name name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public Name Name { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
    }

    private Location(State state) =>
        State = state;

    public State State { get; }

    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern State NewState(Name name, int x, int y);

    public static Req<Input, Location>.Full Transformation =>
        Req<Input, Location>.Ensure<Input>(
            input => input.Name is not null,
            "A location requires an established name.") >>
        Req<Input, Location>.Transform<Input, Location>(
            input => new Location(NewState(input.Name, input.X, input.Y)));

    public static Req<State, Location>.Full Evolution =>
        Req<State, Location>.Ensure<State>(
            state => state.Name is not null,
            "A location must keep an established name.") >>
        Req<State, Location>.Transform<State, Location>(
            state => new Location(state));
}
