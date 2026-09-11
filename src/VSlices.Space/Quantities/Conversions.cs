using System.Numerics;

namespace VSlices.Space.Quantities;

/// <summary>
/// Explicit semantic establishments for VSlices-owned algebraic quantities.
/// This manual surface is intentionally shaped like the code a future source generator may emit.
/// </summary>
public static partial class Conversions
{
    public static Area<LEFT_C, RIGHT_C, T> area<LEFT_C, RIGHT_C, T>(
        Product<
            Dimension.Length,
            LEFT_C,
            Dimension.Length,
            RIGHT_C,
            T> product)
        where LEFT_C : Coordinate<Dimension.Length>
        where RIGHT_C : Coordinate<Dimension.Length>
        where T : INumber<T> =>
        new(product);
}
