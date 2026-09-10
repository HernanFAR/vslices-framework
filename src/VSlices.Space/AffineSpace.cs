using System.Numerics;

namespace VSlices.Space;

/// <summary>
/// Defines an affine space whose values can be translated by a distance and
/// whose subtraction yields a distance in the associated vector space.
/// </summary>
/// <typeparam name="SELF">The point type inhabiting the affine space.</typeparam>
/// <typeparam name="DISTANCE">The vector type describing displacement between points.</typeparam>
/// <typeparam name="DISTANCE_SCALAR">The scalar type used by the distance vector space.</typeparam>
public interface AffineSpace<SELF, DISTANCE, DISTANCE_SCALAR> :
    DiscreteSpace<SELF>,
    IAdditionOperators<SELF, DISTANCE, SELF>,
    ISubtractionOperators<SELF, SELF, DISTANCE>
    where SELF : AffineSpace<SELF, DISTANCE, DISTANCE_SCALAR>
    where DISTANCE : VectorSpace<DISTANCE, DISTANCE_SCALAR>
    where DISTANCE_SCALAR : notnull;
