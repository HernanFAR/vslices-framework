using LanguageExt;

namespace VSlices.Space;

/// <summary>
/// Defines a discrete space whose complete set of recognized values is explicitly maintained.
/// </summary>
/// <typeparam name="SELF">The value type inhabiting the maintained space.</typeparam>
public interface MaintainedSpace<SELF> : DiscreteSpace<SELF>
    where SELF : MaintainedSpace<SELF>
{
    /// <summary>
    /// Gets the complete set of values recognized by this space.
    /// </summary>
    static abstract Seq<SELF> All { get; }
}
