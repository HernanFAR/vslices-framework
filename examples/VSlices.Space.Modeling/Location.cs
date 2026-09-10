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
    public readonly record struct Input(string Name, int X, int Y);

    public sealed record State
    {
        private State(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public string Name { get; init; }
        public int X { get; init; }
        public int Y { get; init; }
    }

    private Location(State state) =>
        State = state;

    public State State { get; }

    public static Req<Input, Location>.Full Transformation =>
        Req<Input, Location>.Ensure<Input>(
            input => !string.IsNullOrWhiteSpace(input.Name),
            "A location requires a non-empty name.") >>
        Req<Input, Location>.Transform<Input, Location>(
            input => new Location(new State(input.Name.Trim(), input.X, input.Y)));

    public static Req<State, Location>.Full Evolution =>
        Req<State, Location>.Ensure<State>(
            state => !string.IsNullOrWhiteSpace(state.Name),
            "A location must keep a non-empty name.") >>
        Req<State, Location>.Transform<State, Location>(
            state => new Location(state));
}
