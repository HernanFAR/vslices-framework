namespace VSlices.Space.Quantities.Abstract;

/// <summary>
/// Marker vocabulary for algebraic exponents used by M.Pow.
/// Concrete exponents are introduced only when pressured by real quantity cases.
/// </summary>
public interface Exponent;

/// <summary>
/// Algebraic exponent two.
/// </summary>
public readonly struct N2 : Exponent;
