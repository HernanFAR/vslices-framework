using System.Numerics;

namespace VSlices.Space.Modeling.CoordinateProductProbe;

// Probe only: collapse Unit + Prefix into one coordinate and require both
// operands of dimensional multiplication to share the same backing type T.

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

public readonly record struct ProductQuantity<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>(T Value) :
    Q<
        Dimension.Product<LEFT_F, RIGHT_F>,
        ProductCoordinate<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C>,
        T>
    where LEFT_F : Dimension
    where LEFT_C : Coordinate<LEFT_F>
    where RIGHT_F : Dimension
    where RIGHT_C : Coordinate<RIGHT_F>
    where T : INumber<T>;

public readonly record struct ProbeMass<T>(T Value) : Q<Dimension.Mass, Kilograms, T>
    where T : INumber<T>;

public readonly record struct ProbeLength<T>(T Value) : Q<Dimension.Length, Meters, T>
    where T : INumber<T>;

public static class ProductOperators
{
    extension<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>(Q<LEFT_F, LEFT_C, T>)
        where LEFT_F : Dimension
        where LEFT_C : Coordinate<LEFT_F>
        where RIGHT_F : Dimension
        where RIGHT_C : Coordinate<RIGHT_F>
        where T : INumber<T>
    {
        public static ProductQuantity<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T> operator *(
            Q<LEFT_F, LEFT_C, T> left,
            Q<RIGHT_F, RIGHT_C, T> right) =>
            new(left.Value * right.Value);
    }
}
