using LanguageExt;
using VSlices.Arrows;

namespace VSlices.Space;

/// <summary>
/// Describes a point whose accepted state can evolve without mutating the source instance.
/// </summary>
/// <typeparam name="SELF">The semantic point type.</typeparam>
/// <typeparam name="STATE">The shape of the point's current state.</typeparam>
public interface Evolvable<SELF, STATE>
    where SELF : Evolvable<SELF, STATE>
{
    /// <summary>
    /// Gets the state currently accepted by this point.
    /// </summary>
    STATE CurrentState { get; }

    /// <summary>
    /// Gets the rules that determine whether a proposed state constitutes a valid next instance.
    /// </summary>
    static abstract Req<STATE, SELF>.Full Evolution { get; }

    /// <summary>
    /// Proposes a transformation over the current state and, when accepted,
    /// materializes a new <typeparamref name="SELF"/> instance.
    /// The source instance is not modified.
    /// </summary>
    Fin<SELF> Update(Func<STATE, STATE> update) =>
        SELF.Evolution.RunFin(update(CurrentState));
}
