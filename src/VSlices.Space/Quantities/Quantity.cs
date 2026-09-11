using System.Numerics;

namespace VSlices.Space.Quantities;

/// <summary>
/// Identifies a dimensional quantity family.
/// </summary>
public abstract class Dimension
{
    private Dimension()
    {
    }

    public sealed class Mass : Dimension
    {
        private Mass()
        {
        }
    }

    public sealed class Length : Dimension
    {
        private Length()
        {
        }
    }

    public sealed class Duration : Dimension
    {
        private Duration()
        {
        }
    }
}

/// <summary>
/// An effective coordinate belonging to a dimensional family.
/// Scale is expressed relative to the family's current canonical coordinate.
/// A coordinate may already encode what would otherwise be modeled as a unit
/// plus a multiplicative prefix.
/// </summary>
public interface Coordinate<F>
    where F : Dimension
{
    static abstract decimal Scale { get; }
}

public readonly struct Grams : Coordinate<Dimension.Mass>
{
    public static decimal Scale => 1m;
}

public readonly struct Kilograms : Coordinate<Dimension.Mass>
{
    public static decimal Scale => 1_000m;
}

public readonly struct Micrograms : Coordinate<Dimension.Mass>
{
    public static decimal Scale => 0.000001m;
}

public readonly struct Pounds : Coordinate<Dimension.Mass>
{
    public static decimal Scale => 453.59237m;
}

public readonly struct Meters : Coordinate<Dimension.Length>
{
    public static decimal Scale => 1m;
}

public readonly struct Kilometers : Coordinate<Dimension.Length>
{
    public static decimal Scale => 1_000m;
}

public readonly struct Micrometers : Coordinate<Dimension.Length>
{
    public static decimal Scale => 0.000001m;
}

public readonly struct Feet : Coordinate<Dimension.Length>
{
    public static decimal Scale => 0.3048m;
}

public readonly struct Seconds : Coordinate<Dimension.Duration>
{
    public static decimal Scale => 1m;
}

public readonly struct Minutes : Coordinate<Dimension.Duration>
{
    public static decimal Scale => 60m;
}

public readonly struct Microseconds : Coordinate<Dimension.Duration>
{
    public static decimal Scale => 0.000001m;
}

/// <summary>
/// Quantity-family membership, effective coordinate, and numeric carrier.
/// </summary>
public interface Q<F, C, T>
    where F : Dimension
    where C : Coordinate<F>
    where T : INumberBase<T>
{
    T Value { get; }
}
