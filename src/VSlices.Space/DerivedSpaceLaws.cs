namespace VSlices.Space;

/// <summary>
/// Defines the semantic laws that govern <see cref="DerivedSpace{SELF, BASE}"/>.
/// </summary>
/// <remarks>
/// <para>
/// A derived space is a semantic subset of its base space. These laws describe
/// the meaning of that relationship independently of any particular realization.
/// </para>
/// <list type="number">
/// <item>
/// <description>
/// <b>Subset law.</b> Every value of the derived space is also semantically a valid
/// value of the base space.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Total widening.</b> Every derived value can widen to the base space. Widening
/// must not require additional evidence and must not fail.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Base-semantic preservation.</b> Widening may forget semantics introduced by
/// the derived space, but it must not alter the semantics already recognized by
/// the base space.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Equality preservation.</b> If two values are equal according to the derived
/// space, their widened values must be equal according to the base space. The
/// converse is not required.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Transitive inclusion.</b> If C derives from B and B derives from A, then C is
/// also semantically contained in A.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>Transitive widening coherence.</b> Widening through intermediate derived
/// spaces must preserve the same base semantics as widening directly to the more
/// general base whenever such a direct widening is available.
/// </description>
/// </item>
/// <item>
/// <description>
/// <b>No inverse law.</b> Derivation does not imply a total narrowing from the base
/// space into the derived space. Establishing a base value as a derived value may
/// require additional invariants or evidence and may fail.
/// </description>
/// </item>
/// </list>
/// </remarks>
public static class DerivedSpaceLaws;
