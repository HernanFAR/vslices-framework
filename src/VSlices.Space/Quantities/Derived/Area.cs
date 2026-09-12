using System.Numerics;
using VSlices.Space.Quantities.Abstract;

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

/// <summary>
/// Explicitly authorized Area x Length multiplication.
/// The incoming Length is converted to the Area's primitive Length basis before
/// multiplication. The result keeps the full structural Product shape because
/// Area and Length have different dimensional shapes, even though their primitive
/// coordinate basis can be aligned.
/// </summary>
public static class AreaProductOperators
{
    extension<LEFT_C, RIGHT_C, T, RIGHT_SELF>(Area<LEFT_C, T>)
        where LEFT_C : Coordinate<Dimension.Length>
        where RIGHT_C : Coordinate<Dimension.Length>
        where T : INumber<T>
        where RIGHT_SELF : Length<RIGHT_C, T, RIGHT_SELF>
    {
        public static Product<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Dimension.Length, LEFT_C>,
            Dimension.Length,
            LEFT_C,
            T> operator *(
            Area<LEFT_C, T> left,
            Length<RIGHT_C, T, RIGHT_SELF> right)
        {
            var rightValue = T.CreateChecked(right.Value);
            var rightScale = T.CreateChecked(RIGHT_C.Scale);
            var leftScale = T.CreateChecked(LEFT_C.Scale);
            var rightInLeftCoordinate = rightValue * rightScale / leftScale;

            return new(left.Value * rightInLeftCoordinate);
        }
    }
}
