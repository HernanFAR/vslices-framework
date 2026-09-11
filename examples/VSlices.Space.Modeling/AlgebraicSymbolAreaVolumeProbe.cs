using System.Numerics;

namespace VSlices.Space.Modeling.AlgebraicSymbolAreaVolumeProbe;

// Probe only. This file intentionally models the C# surface that a future
// source generator could emit from semantic declarations. The purpose is to
// pressure the language shape before committing to a generator design.

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class AlgebraicSymbolAttribute(string symbol) : Attribute
{
    public string Symbol { get; } = symbol;
}

public interface AlgebraicQuantity<T>
    where T : INumber<T>
{
    T Value { get; }
}

public readonly record struct Length<T>(T Value) : AlgebraicQuantity<T>
    where T : INumber<T>;

/// <summary>
/// Canonical structural result of an explicitly available multiplication.
/// A and B remain part of the type so the product carries its algebraic origin;
/// T is the converged carrier used by both operands.
/// </summary>
public sealed record Product<A, B, T>(T Value) : AlgebraicQuantity<T>
    where A : AlgebraicQuantity<T>
    where B : AlgebraicQuantity<T>
    where T : INumber<T>;

/// <summary>
/// Local stand-in for the relation currently expressed by DerivedSpace.
/// It keeps this probe focused on the algebraic/generated C# surface.
/// </summary>
public interface DerivedFrom<BASE>
{
    BASE ToBase();
}

[AlgebraicSymbol("area")]
public sealed record Area<T>(Product<Length<T>, Length<T>, T> Product) :
    AlgebraicQuantity<T>,
    DerivedFrom<Product<Length<T>, Length<T>, T>>
    where T : INumber<T>
{
    public T Value => Product.Value;
    public Product<Length<T>, Length<T>, T> ToBase() => Product;
}

[AlgebraicSymbol("volume")]
public sealed record Volume<T>(Product<Area<T>, Length<T>, T> Product) :
    AlgebraicQuantity<T>,
    DerivedFrom<Product<Area<T>, Length<T>, T>>
    where T : INumber<T>
{
    public T Value => Product.Value;
    public Product<Area<T>, Length<T>, T> ToBase() => Product;
}

/// <summary>
/// Stand-in for members that a source generator could emit after observing the
/// declared Area and Volume algebraic relations. Notice that multiplication is
/// not universal over AlgebraicQuantity: only the two declared operand pairs
/// receive operator syntax.
/// </summary>
public static class GeneratedProductOperators
{
    extension<T>(Length<T>)
        where T : INumber<T>
    {
        public static Product<Length<T>, Length<T>, T> operator *(
            Length<T> left,
            Length<T> right) =>
            new(left.Value * right.Value);
    }

    extension<T>(Area<T>)
        where T : INumber<T>
    {
        public static Product<Area<T>, Length<T>, T> operator *(
            Area<T> left,
            Length<T> right) =>
            new(left.Value * right.Value);
    }
}

/// <summary>
/// Stand-in for generated VSlices-owned conversion symbols. Consumer-owned
/// declarations would target a separately configured CustomConversions class.
/// </summary>
public static partial class Conversions
{
    public static Area<T> area<T>(Product<Length<T>, Length<T>, T> product)
        where T : INumber<T> =>
        new(product);

    public static Volume<T> volume<T>(Product<Area<T>, Length<T>, T> product)
        where T : INumber<T> =>
        new(product);
}

#if MODELING_INVALID_USAGE
public static class InvalidUndeclaredProductUsage
{
    public static void MustNotCompile()
    {
        var area = new Area<double>(new Product<Length<double>, Length<double>, double>(4d));

        // No multiplication was declared/generated for Area * Area. If this
        // compiles, the probe accidentally introduced universal Product syntax.
        _ = area * area;
    }
}
#endif
