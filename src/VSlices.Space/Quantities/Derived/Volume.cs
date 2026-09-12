using System.Numerics;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Semantic volume established from Area x Length.
/// C remains the primitive Length coordinate basis; the cubic shape lives entirely
/// in nested M.Mul composition.
/// </summary>
[AlgebraicSymbol("volume")]
public sealed record Volume<C, T>(
    Product<M.Mul<M.Length, M.Length>, M.Length, C, T> Product) :
    Q<M.Mul<M.Mul<M.Length, M.Length>, M.Length>, C, T>,
    DerivedSpace<
        Volume<C, T>,
        Product<M.Mul<M.Length, M.Length>, M.Length, C, T>>
    where C : Coordinate<M.Length>
    where T : INumber<T>
{
    public T Value => Product.Value;

    public Product<M.Mul<M.Length, M.Length>, M.Length, C, T> ToBase() => Product;
}
