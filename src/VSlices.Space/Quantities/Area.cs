using System.Numerics;

namespace VSlices.Space.Quantities;

/// <summary>
/// Semantic area established from a homogeneous Length product.
/// C names the shared Length coordinate basis used to express the composed
/// dimensional shape. An Area expressed over Kilometers is therefore read in
/// square kilometers without requiring a separate SquaredKilometers type.
/// </summary>
[AlgebraicSymbol("area")]
public sealed record Area<C, T>(
    Product<
        Dimension.Length,
        Dimension.Length,
        C,
        T> Product) :
    Q<
        Dimension.Product<Dimension.Length, Dimension.Length>,
        ProductCoordinate<Dimension.Length, Dimension.Length, C>,
        T>,
    DerivedSpace<
        Area<C, T>,
        Product<Dimension.Length, Dimension.Length, C, T>>
    where C : Coordinate<Dimension.Length>
    where T : INumber<T>
{
    public T Value => Product.Value;

    public Product<Dimension.Length, Dimension.Length, C, T> ToBase() =>
        Product;
}
