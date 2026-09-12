namespace VSlices.Space.Quantities;

/// <summary>
/// Names the explicit algebraic establishment function associated with a semantic quantity.
/// Tooling may use this declaration to materialize syntax, but the structural relation must
/// already be expressed by the quantity type itself.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class AlgebraicSymbolAttribute(string symbol) : Attribute
{
    public string Symbol { get; } = symbol;
}
