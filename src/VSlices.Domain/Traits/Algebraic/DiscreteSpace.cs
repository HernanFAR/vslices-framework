using System;
using System.Numerics;

namespace VSlices.Domain.Traits;

/// <summary>
/// Defines a semantic space whose values support equality comparison.
/// Equality semantics are defined by the space and must reflect its intended
/// notion of identity or equivalence.
/// This contract establishes equality only; it does not imply ordering,
/// arithmetic operations, composition, or any other algebraic structure.
/// </summary>
/// <typeparam name="SELF">
/// The value type that inhabits this semantic space and implements its equality semantics.
/// </typeparam>
public interface DiscreteSpace<SELF> :
    IEquatable<SELF>,
    IEqualityOperators<SELF, SELF, bool>
    where SELF : DiscreteSpace<SELF>;
