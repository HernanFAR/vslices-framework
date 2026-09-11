using System.Numerics;

namespace VSlices.Space.Quantities;

/// <summary>
/// Explicit semantic establishments for VSlices-owned algebraic quantities.
/// This manual surface is intentionally shaped like the code a future source generator may emit.
/// </summary>
public static partial class Conversions
{
    public static Area<C, T> area<C, T>(
        Product<
            Dimension.Length,
            Dimension.Length,
            C,
            T> product)
        where C : Coordinate<Dimension.Length>
        where T : INumber<T> =>
        new(product);
}
