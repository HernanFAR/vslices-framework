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

    /// <summary>
    /// Structural dimensional multiplication. The operands remain visible in
    /// the type; no semantic interpretation such as Area or Energy is implied.
    /// </summary>
    public sealed class Product<LEFT, RIGHT> : Dimension
        where LEFT : Dimension
        where RIGHT : Dimension
    {
        private Product()
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
/// Effective coordinate produced by multiplying two quantity coordinates.
/// The coordinate keeps the dimensional families explicit because C# cannot
/// recover them as associated types from LEFT_C and RIGHT_C alone.
/// </summary>
public readonly struct ProductCoordinate<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C> :
    Coordinate<Dimension.Product<LEFT_F, RIGHT_F>>
    where LEFT_F : Dimension
    where LEFT_C : Coordinate<LEFT_F>
    where RIGHT_F : Dimension
    where RIGHT_C : Coordinate<RIGHT_F>
{
    public static decimal Scale => LEFT_C.Scale * RIGHT_C.Scale;
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
