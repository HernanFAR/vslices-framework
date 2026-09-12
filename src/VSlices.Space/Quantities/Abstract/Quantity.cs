using System.Numerics;

namespace VSlices.Space.Quantities.Abstract;

/// <summary>
/// Quantity-family membership, effective coordinate, and numeric carrier.
/// </summary>
public interface Q<F, C, T>
    where F : Dimension
    where C : Coordinate<F>
    where T : INumberBase<T>
{
    T Value { get; }
}

