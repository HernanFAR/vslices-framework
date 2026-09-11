using System.Numerics;

namespace VSlices.Space.Quantities;

/// <summary>
/// Semantic area established from the structural product of two Length quantities.
/// The exact effective coordinates of both operands are preserved.
/// </summary>
[AlgebraicSymbol("area")]
public sealed record Area<LEFT_C, RIGHT_C, T>(
    Product<
        Dimension.Length,
        LEFT_C,
        Dimension.Length,
        RIGHT_C,
        T> Product) :
    Q<
        Dimension.Product<Dimension.Length, Dimension.Length>,
        ProductCoordinate<Dimension.Length, LEFT_C, Dimension.Length, RIGHT_C>,
        T>,
    DerivedSpace<
        Area<LEFT_C, RIGHT_C, T>,
        Product<Dimension.Length, LEFT_C, Dimension.Length, RIGHT_C, T>>
    where LEFT_C : Coordinate<Dimension.Length>
    where RIGHT_C : Coordinate<Dimension.Length>
    where T : INumber<T>
{
    public T Value => Product.Value;

    public Product<Dimension.Length, LEFT_C, Dimension.Length, RIGHT_C, T> ToBase() =>
        Product;
}
