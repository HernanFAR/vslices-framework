using System.Numerics;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Structural exponentiation of one magnitude over one primitive coordinate basis.
/// The algebraic shape lives in M.Pow; C remains the primitive basis used to express
/// the powered magnitude.
/// </summary>
public sealed record Power<BASE_F, EXPONENT, C, T>(T Value) :
    Q<M.Pow<BASE_F, EXPONENT>, C, T>,
    DiscreteSpace<Power<BASE_F, EXPONENT, C, T>>
    where BASE_F : M
    where EXPONENT : Exponent
    where C : Coordinate
    where T : INumber<T>;

/// <summary>
/// First production pressure for Power: squaring a Length.
/// C# has no exponentiation operator, so the structural operation is named explicitly.
/// </summary>
public static class PowerOperations
{
    public static Power<M.Length, N2, C, T> square<SELF, C, T>(
        Length<SELF, C, T> value)
        where SELF : Length<SELF, C, T>
        where C : Coordinate<M.Length>
        where T : INumber<T> =>
        new(value.Value * value.Value);
}
