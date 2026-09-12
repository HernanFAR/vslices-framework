using System.Numerics;
using VSlices.Space.Quantities;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space;

/// <summary>
/// Explicit semantic establishments for VSlices-owned algebraic quantities.
/// This manual surface is intentionally shaped like the code a future source generator may emit.
/// </summary>
public static partial class Conversions
{
    public static Area<C, T> area<C, T>(
        Product<M.Length, M.Length, C, T> product)
        where C : Coordinate<M.Length>
        where T : INumber<T> =>
        new(product);

    public static Volume<C, T> volume<C, T>(
        Product<M.Mul<M.Length, M.Length>, M.Length, C, T> product)
        where C : Coordinate<M.Length>
        where T : INumber<T> =>
        new(product);
}
