using System.Numerics;

namespace VSlices.Domain.Traits;

/// <summary>
///
/// </summary>
/// <typeparam name="SELF">
///
/// </typeparam>
/// <typeparam name="SCALAR">
///
/// </typeparam>
/// <remarks>
///
/// </remarks>
public interface Magnitude<SELF, SCALAR> :
    DomainType<SELF>,
    VectorSpace<SELF, SCALAR>,
    IComparable<SELF>,
    IComparisonOperators<SELF, SELF, bool>
    where SELF : Magnitude<SELF, SCALAR>
    where SCALAR : notnull;
