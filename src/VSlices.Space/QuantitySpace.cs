using System.Numerics;

namespace VSlices.Space;

/// <summary>
/// Defines an ordered quantitative space with a canonical scalar coordinate.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="CanonValue"/> is semantic: it is the coordinate chosen by the
/// quantity space as its canonical metric. Other units, scales, or external
/// representations may relate to that coordinate without becoming part of the
/// space's identity.
/// </para>
/// </remarks>
/// <typeparam name="SELF">The quantity type inhabiting the space.</typeparam>
/// <typeparam name="SCALAR">The scalar used by the quantity space.</typeparam>
public interface QuantitySpace<SELF, SCALAR> :
    VectorSpace<SELF, SCALAR>,
    IComparable<SELF>,
    IComparisonOperators<SELF, SELF, bool>
    where SELF : QuantitySpace<SELF, SCALAR>
    where SCALAR : notnull
{
    /// <summary>
    /// Gets the canonical scalar coordinate of this quantity within its space.
    /// </summary>
    SCALAR CanonValue { get; }
}
