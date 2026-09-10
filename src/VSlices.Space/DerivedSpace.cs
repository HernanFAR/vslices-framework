using VSlices.Space.Derived;

namespace VSlices.Space;

/// <summary>
/// Defines a discrete space whose values form a semantic subset of another discrete space.
/// </summary>
/// <remarks>
/// The relationship is governed by <see cref="DerivedSpaceLaws"/>.
/// </remarks>
/// <typeparam name="SELF">The derived value type.</typeparam>
/// <typeparam name="BASE">The base value type whose semantic space contains the derived space.</typeparam>
public interface DerivedSpace<SELF, BASE> : DiscreteSpace<SELF>
    where SELF : DerivedSpace<SELF, BASE>
    where BASE : DiscreteSpace<BASE>
{
    /// <summary>
    /// Widens this value into its base space.
    /// </summary>
    BASE ToBase();
}
