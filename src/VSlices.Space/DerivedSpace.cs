namespace VSlices.Space;

/// <summary>
/// Defines a discrete space whose values derive from values in another discrete space.
/// </summary>
/// <typeparam name="SELF">The derived value type.</typeparam>
/// <typeparam name="BASE">The base value type from which this space derives.</typeparam>
public interface DerivedSpace<SELF, BASE> : DiscreteSpace<SELF>
    where SELF : DerivedSpace<SELF, BASE>
    where BASE : DiscreteSpace<BASE>
{
    /// <summary>
    /// Projects this derived value into its base space.
    /// </summary>
    BASE ToBase();
}
