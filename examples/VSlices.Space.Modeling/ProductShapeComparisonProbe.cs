using System.Numerics;

namespace VSlices.Space.Modeling.ProductShapeComparisonProbe;

// Probe only. Compare three ways of representing the result of dimensional
// multiplication while preserving two decisions from the current exploration:
// 1. composed coordinates retain the effective units used by the operands;
// 2. both operands converge on one shared backing type T.

public abstract class Dimension
{
    private Dimension() { }

    public sealed class Mass : Dimension { private Mass() { } }
    public sealed class Length : Dimension { private Length() { } }

    public sealed class Product<LEFT, RIGHT> : Dimension
        where LEFT : Dimension
        where RIGHT : Dimension
    {
        private Product() { }
    }
}

public interface Coordinate<F>
    where F : Dimension
{
    static abstract decimal Scale { get; }
}

public readonly struct Kilograms : Coordinate<Dimension.Mass>
{
    public static decimal Scale => 1_000m;
}

public readonly struct Meters : Coordinate<Dimension.Length>
{
    public static decimal Scale => 1m;
}

public readonly struct ProductCoordinate<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C> :
    Coordinate<Dimension.Product<LEFT_F, RIGHT_F>>
    where LEFT_F : Dimension
    where LEFT_C : Coordinate<LEFT_F>
    where RIGHT_F : Dimension
    where RIGHT_C : Coordinate<RIGHT_F>
{
    public static decimal Scale => LEFT_C.Scale * RIGHT_C.Scale;
}

public interface Q<F, C, T>
    where F : Dimension
    where C : Coordinate<F>
    where T : INumberBase<T>
{
    T Value { get; }
}

public readonly record struct ProbeMass<T>(T Value) : Q<Dimension.Mass, Kilograms, T>
    where T : INumber<T>;

public readonly record struct ProbeLength<T>(T Value) : Q<Dimension.Length, Meters, T>
    where T : INumber<T>;

// A — current probe shape. Product owns the decomposition of both operands.
public readonly record struct ProductA<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>(T Value) :
    Q<
        Dimension.Product<LEFT_F, RIGHT_F>,
        ProductCoordinate<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C>,
        T>
    where LEFT_F : Dimension
    where LEFT_C : Coordinate<LEFT_F>
    where RIGHT_F : Dimension
    where RIGHT_C : Coordinate<RIGHT_F>
    where T : INumber<T>;

// B — option 5. Product receives the already-composed family and coordinate.
// It no longer owns or remembers the decomposition that produced them.
public abstract class ProductB<F, C, T, SELF> : Q<F, C, T>
    where F : Dimension
    where C : Coordinate<F>
    where T : INumber<T>
    where SELF : ProductB<F, C, T, SELF>
{
    protected ProductB(T value) => Value = value;

    public T Value { get; }
}

public sealed class ProductB<F, C, T> : ProductB<F, C, T, ProductB<F, C, T>>
    where F : Dimension
    where C : Coordinate<F>
    where T : INumber<T>
{
    public ProductB(T value) : base(value) { }
}

// C — stronger erasure candidate. There is no quantity-level Product family at
// all; Product survives only in Dimension and Coordinate. The result is just a
// structural quantity living at the composed F/C coordinate.
public readonly record struct StructuralQuantity<F, C, T>(T Value) : Q<F, C, T>
    where F : Dimension
    where C : Coordinate<F>
    where T : INumber<T>;

public static class ProductComparison
{
    public static ProductA<Dimension.Mass, Kilograms, Dimension.Length, Meters, T> MultiplyA<T>(
        ProbeMass<T> left,
        ProbeLength<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);

    public static ProductB<
        Dimension.Product<Dimension.Mass, Dimension.Length>,
        ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
        T> MultiplyB<T>(
        ProbeMass<T> left,
        ProbeLength<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);

    public static StructuralQuantity<
        Dimension.Product<Dimension.Mass, Dimension.Length>,
        ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
        T> MultiplyC<T>(
        ProbeMass<T> left,
        ProbeLength<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);

    // First depth increase: Length × Length.
    public static ProductA<Dimension.Length, Meters, Dimension.Length, Meters, T> SquareA<T>(
        ProbeLength<T> left,
        ProbeLength<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);

    public static ProductB<
        Dimension.Product<Dimension.Length, Dimension.Length>,
        ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
        T> SquareB<T>(
        ProbeLength<T> left,
        ProbeLength<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);

    public static StructuralQuantity<
        Dimension.Product<Dimension.Length, Dimension.Length>,
        ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
        T> SquareC<T>(
        ProbeLength<T> left,
        ProbeLength<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);

    // Second depth increase: (Length × Length) × Mass.
    // A must feed its previous decomposition back as the next left family/coordinate pair.
    public static ProductA<
        Dimension.Product<Dimension.Length, Dimension.Length>,
        ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
        Dimension.Mass,
        Kilograms,
        T> NestedA<T>(
        ProductA<Dimension.Length, Meters, Dimension.Length, Meters, T> left,
        ProbeMass<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);

    // B keeps the same four-parameter quantity family; nesting grows only inside F and C.
    public static ProductB<
        Dimension.Product<Dimension.Product<Dimension.Length, Dimension.Length>, Dimension.Mass>,
        ProductCoordinate<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
            Dimension.Mass,
            Kilograms>,
        T> NestedB<T>(
        ProductB<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
            T> left,
        ProbeMass<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);

    // C has the same composed F/C growth as B, but no quantity-level Product family.
    public static StructuralQuantity<
        Dimension.Product<Dimension.Product<Dimension.Length, Dimension.Length>, Dimension.Mass>,
        ProductCoordinate<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
            Dimension.Mass,
            Kilograms>,
        T> NestedC<T>(
        StructuralQuantity<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
            T> left,
        ProbeMass<T> right)
        where T : INumber<T> =>
        new(left.Value * right.Value);
}
