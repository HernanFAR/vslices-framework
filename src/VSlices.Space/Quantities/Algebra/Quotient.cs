using System.Numerics;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Structural division whose numerator and denominator retain their own primitive
/// coordinate bases. Unlike the compact Product case, Length / Duration does not
/// have one truthful coordinate C: km/h is intrinsically expressed by two bases.
/// </summary>
public sealed record Quotient<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>(T Value) :
    DiscreteSpace<Quotient<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>>
    where LEFT_F : M
    where LEFT_C : Coordinate
    where RIGHT_F : M
    where RIGHT_C : Coordinate
    where T : INumber<T>;

/// <summary>
/// Explicitly authorized Length / Duration division.
/// The result preserves both primitive coordinate bases instead of normalizing
/// them into an invented single coordinate.
/// </summary>
public static class LengthDurationQuotientOperators
{
    extension<LEFT_SELF, LEFT_C, RIGHT_SELF, RIGHT_C, T>(Length<LEFT_SELF, LEFT_C, T>)
        where LEFT_SELF : Length<LEFT_SELF, LEFT_C, T>
        where LEFT_C : Coordinate<M.Length>
        where RIGHT_SELF : Duration<RIGHT_SELF, RIGHT_C, T>
        where RIGHT_C : Coordinate<M.Duration>
        where T : INumber<T>
    {
        public static Quotient<M.Length, LEFT_C, M.Duration, RIGHT_C, T> operator /(
            Length<LEFT_SELF, LEFT_C, T> left,
            Duration<RIGHT_SELF, RIGHT_C, T> right) =>
            new(left.Value / right.Value);
    }
}
