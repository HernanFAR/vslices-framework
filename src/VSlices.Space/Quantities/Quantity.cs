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
}

/// <summary>
/// A unit belonging to a dimensional family.
/// Scale is expressed relative to the family's current reference unit.
/// </summary>
public interface Unit<F>
    where F : Dimension
{
    static abstract decimal Scale { get; }
}

public readonly struct Grams : Unit<Dimension.Mass>
{
    public static decimal Scale => 1m;
}

public readonly struct Pounds : Unit<Dimension.Mass>
{
    public static decimal Scale => 453.59237m;
}

/// <summary>
/// A multiplicative prefix applied to a unit coordinate.
/// </summary>
public interface Prefix
{
    static abstract decimal Scale { get; }
}

public readonly struct None : Prefix
{
    public static decimal Scale => 1m;
}

public readonly struct Kilo : Prefix
{
    public static decimal Scale => 1_000m;
}

public readonly struct Micro : Prefix
{
    public static decimal Scale => 0.000001m;
}

/// <summary>
/// Quantity-family membership and representation coordinate.
/// </summary>
public interface Q<F, U, P, T>
    where F : Dimension
    where U : Unit<F>
    where P : Prefix
    where T : INumberBase<T>
{
    T Value { get; }
}
