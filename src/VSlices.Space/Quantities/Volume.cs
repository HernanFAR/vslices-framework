using System.Numerics;

namespace VSlices.Space.Quantities;

/// <summary>
/// Semantic volume established from an Area multiplied by a Length.
/// C names the shared primitive Length coordinate basis used to express the
/// composed dimensional shape. A Volume expressed over Kilometers is therefore
/// read in cubic kilometers without requiring a separate CubicKilometers type.
/// </summary>
[AlgebraicSymbol("volume")]
public sealed record Volume<C, T>(
    Product<
        Dimension.Product<Dimension.Length, Dimension.Length>,
        ProductCoordinate<Dimension.Length, Dimension.Length, C>,
        Dimension.Length,
        C,
        T> Product) :
    Q<
        Dimension.Product<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            Dimension.Length>,
        ProductCoordinate<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Dimension.Length, C>,
            Dimension.Length,
            C>,
        T>,
    DerivedSpace<
        Volume<C, T>,
        Product<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Dimension.Length, C>,
            Dimension.Length,
            C,
            T>>
    where C : Coordinate<Dimension.Length>
    where T : INumber<T>
{
    public T Value => Product.Value;

    public Product<
        Dimension.Product<Dimension.Length, Dimension.Length>,
        ProductCoordinate<Dimension.Length, Dimension.Length, C>,
        Dimension.Length,
        C,
        T> ToBase() =>
        Product;
}
