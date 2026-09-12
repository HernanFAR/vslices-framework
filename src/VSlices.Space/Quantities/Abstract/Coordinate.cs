namespace VSlices.Space.Quantities.Abstract;

/// <summary>
/// Primitive measurement basis for a quantity value.
///
/// ReferenceScale is oriented as:
///
///   1 coordinate unit = ReferenceScale * canonical reference unit
///
/// The current canonical references are fixed by VSlices and are not configurable:
/// Mass -> Gram, Length -> Meter, Duration -> Second.
/// </summary>
public interface Coordinate
{
    static abstract decimal ReferenceScale { get; }
}

/// <summary>
/// Declares the primitive magnitude family to which a coordinate belongs.
/// Composed magnitudes may reuse a primitive coordinate basis without manufacturing
/// synthetic coordinates such as SquaredKilometers or CubicMeters.
/// </summary>
public interface Coordinate<F> : Coordinate
    where F : M;

public readonly struct Grams : Coordinate<M.Mass>
{
    public static decimal ReferenceScale => 1m;
}

public readonly struct Kilograms : Coordinate<M.Mass>
{
    public static decimal ReferenceScale => 1_000m;
}

public readonly struct Micrograms : Coordinate<M.Mass>
{
    public static decimal ReferenceScale => 0.000001m;
}

public readonly struct Pounds : Coordinate<M.Mass>
{
    public static decimal ReferenceScale => 453.59237m;
}

public readonly struct Meters : Coordinate<M.Length>
{
    public static decimal ReferenceScale => 1m;
}

public readonly struct Kilometers : Coordinate<M.Length>
{
    public static decimal ReferenceScale => 1_000m;
}

public readonly struct Micrometers : Coordinate<M.Length>
{
    public static decimal ReferenceScale => 0.000001m;
}

public readonly struct Feet : Coordinate<M.Length>
{
    public static decimal ReferenceScale => 0.3048m;
}

public readonly struct Seconds : Coordinate<M.Duration>
{
    public static decimal ReferenceScale => 1m;
}

public readonly struct Minutes : Coordinate<M.Duration>
{
    public static decimal ReferenceScale => 60m;
}

public readonly struct Microseconds : Coordinate<M.Duration>
{
    public static decimal ReferenceScale => 0.000001m;
}
