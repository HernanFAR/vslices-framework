using System.Numerics;

namespace VSlices.Space.Quantities.Abstract;

/// <summary>
/// Quantity membership: algebraic magnitude F, one primitive coordinate basis C,
/// and numeric carrier T.
///
/// C# does not express every valid relation between a composed magnitude and its
/// primitive basis without reifying a parallel coordinate algebra. Concrete semantic
/// quantity types retain the strongest family constraints they can express directly;
/// tooling/analyzers may verify remaining coherence rules.
/// </summary>
public interface Q<F, C, T>
    where F : M
    where C : Coordinate
    where T : INumberBase<T>
{
    T Value { get; }
}
