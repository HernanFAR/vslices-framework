using System.Numerics;

namespace VSlices.Space;

/// <summary>
/// Defines a vector space whose values support additive composition,
/// subtraction, unary negation, and scalar multiplication and division.
/// </summary>
/// <typeparam name="SELF">The value type inhabiting the vector space.</typeparam>
/// <typeparam name="SCALAR">The scalar type used by the vector space.</typeparam>
public interface VectorSpace<SELF, SCALAR> :
    DiscreteSpace<SELF>,
    IUnaryNegationOperators<SELF, SELF>,
    IAdditiveIdentity<SELF, SELF>,
    IAdditionOperators<SELF, SELF, SELF>,
    ISubtractionOperators<SELF, SELF, SELF>,
    IMultiplyOperators<SELF, SCALAR, SELF>,
    IDivisionOperators<SELF, SCALAR, SELF>
    where SELF : VectorSpace<SELF, SCALAR>
    where SCALAR : notnull
{
    /// <summary>
    /// Returns the origin of the vector space.
    /// </summary>
    public static virtual SELF Origin => SELF.AdditiveIdentity;
}
