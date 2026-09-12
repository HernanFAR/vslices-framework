using System.Numerics;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Semantic speed established from Length / Duration.
///
/// Speed deliberately preserves both primitive coordinate bases, for example
/// Kilometers and Hours for km/h. It therefore does not currently implement
/// Q&lt;F,C,T&gt;, which requires one coordinate C. This is intentional pressure on
/// whether Q is a universal quantity abstraction or only one useful quantity shape.
/// </summary>
[AlgebraicSymbol("speed")]
public sealed record Speed<LENGTH_C, DURATION_C, T>(
    Quotient<M.Length, LENGTH_C, M.Duration, DURATION_C, T> Quotient) :
    DerivedSpace<
        Speed<LENGTH_C, DURATION_C, T>,
        Quotient<M.Length, LENGTH_C, M.Duration, DURATION_C, T>>
    where LENGTH_C : Coordinate<M.Length>
    where DURATION_C : Coordinate<M.Duration>
    where T : INumber<T>
{
    public T Value => Quotient.Value;

    public Quotient<M.Length, LENGTH_C, M.Duration, DURATION_C, T> ToBase() =>
        Quotient;
}
