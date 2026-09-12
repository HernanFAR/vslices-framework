namespace VSlices.Space.Modeling.Quantities;

/// <summary>
/// Defines a multiplicative scale relative to an implicit base coordinate.
/// This is intentionally a modeling probe; Unit has not been introduced yet.
/// </summary>
public interface Prefix<PREFIX>
    where PREFIX : struct, Prefix<PREFIX>
{
    static abstract double Scale { get; }
}

public readonly struct One : Prefix<One>
{
    public static double Scale => 1d;
}

public readonly struct Kilo : Prefix<Kilo>
{
    public static double Scale => 1_000d;
}

public readonly struct Milli : Prefix<Milli>
{
    public static double Scale => 1e-3;
}

public readonly struct Micro : Prefix<Micro>
{
    public static double Scale => 1e-6;
}

/// <summary>
/// Nominal mass dimension used by the quantity mechanism probe.
/// </summary>
public readonly struct MassDimension
{
}

/// <summary>
/// Structural quantity identified by a dimension and a canonical scale.
/// The nominal semantic Value built on top of it remains a distinct type.
/// </summary>
public readonly record struct Quantity<DIM, PREFIX>(double CanonValue) :
    VectorSpace<Quantity<DIM, PREFIX>, double>
    where PREFIX : struct, Prefix<PREFIX>
{
    public static Quantity<DIM, PREFIX> AdditiveIdentity =>
        new(0d);

    public static Quantity<DIM, PREFIX> operator +(
        Quantity<DIM, PREFIX> left,
        Quantity<DIM, PREFIX> right) =>
        new(left.CanonValue + right.CanonValue);

    public static Quantity<DIM, PREFIX> operator -(
        Quantity<DIM, PREFIX> left,
        Quantity<DIM, PREFIX> right) =>
        new(left.CanonValue - right.CanonValue);

    public static Quantity<DIM, PREFIX> operator -(
        Quantity<DIM, PREFIX> value) =>
        new(-value.CanonValue);

    public static Quantity<DIM, PREFIX> operator *(
        Quantity<DIM, PREFIX> quantity,
        double scalar) =>
        new(quantity.CanonValue * scalar);

    public static Quantity<DIM, PREFIX> operator /(
        Quantity<DIM, PREFIX> quantity,
        double scalar) =>
        new(quantity.CanonValue / scalar);

    /// <summary>
    /// Expresses the same dimensional quantity using another prefix.
    /// </summary>
    public Quantity<DIM, TO_PREFIX> In<TO_PREFIX>()
        where TO_PREFIX : struct, Prefix<TO_PREFIX> =>
        new(CanonValue * PREFIX.Scale / TO_PREFIX.Scale);
}
