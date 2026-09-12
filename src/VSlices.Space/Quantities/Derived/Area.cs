using System.Numerics;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Semantic area established from Length x Length.
/// C remains a primitive Length coordinate basis; the square lives in M.Mul.
/// </summary>
[AlgebraicSymbol("area")]
public sealed record Area<C, T>(
    Product<M.Length, M.Length, C, T> Product) :
    Q<M.Mul<M.Length, M.Length>, C, T>,
    DerivedSpace<Area<C, T>, Product<M.Length, M.Length, C, T>>
    where C : Coordinate<M.Length>
    where T : INumber<T>
{
    public T Value => Product.Value;

    public Product<M.Length, M.Length, C, T> ToBase() => Product;
}

/// <summary>
/// Explicitly authorized Area x Length multiplication.
/// RIGHT is converted to the Area's primitive Length basis before multiplication.
/// Since both operands can be expressed on that shared primitive basis, the result
/// uses the compact Product form even though their magnitude shapes differ.
/// </summary>
public static class AreaProductOperators
{
    extension<LEFT_C, RIGHT_SELF, RIGHT_C, T>(Area<LEFT_C, T>)
        where LEFT_C : Coordinate<M.Length>
        where RIGHT_SELF : Length<RIGHT_SELF, RIGHT_C, T>
        where RIGHT_C : Coordinate<M.Length>
        where T : INumber<T>
    {
        public static Product<M.Mul<M.Length, M.Length>, M.Length, LEFT_C, T> operator *(
            Area<LEFT_C, T> left,
            Length<RIGHT_SELF, RIGHT_C, T> right)
        {
            var rightValue = T.CreateChecked(right.Value);
            var rightScale = T.CreateChecked(RIGHT_C.ReferenceScale);
            var leftScale = T.CreateChecked(LEFT_C.ReferenceScale);
            var rightInLeftCoordinate = rightValue * rightScale / leftScale;

            return new(left.Value * rightInLeftCoordinate);
        }
    }
}
