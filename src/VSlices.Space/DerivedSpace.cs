namespace VSlices.Space;

/// <summary>
/// Defines a discrete space whose values form a semantic subset of another discrete space.
/// Every value in the derived space must correspond to a valid value in the base space.
/// Widening a derived value to its base space must be total and preserve its base semantics.
/// </summary>
/// <typeparam name="SELF">The derived value type.</typeparam>
/// <typeparam name="BASE">The base value type whose semantic space contains the derived space.</typeparam>
public interface DerivedSpace<SELF, BASE> : DiscreteSpace<SELF>
    where SELF : DerivedSpace<SELF, BASE>
    where BASE : DiscreteSpace<BASE>
{
    /// <summary>
    /// Widens this value into its base space without failure or loss of base semantics.
    /// </summary>
    BASE ToBase();
}
