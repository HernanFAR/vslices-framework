using System.Numerics;

namespace VSlices.Space.Modeling.QuantityFamilies.Formalization;

/// <summary>
/// Type-level construction authority for a nominal quantity value.
/// </summary>
public interface QuantityFactory<T, SELF>
    where T : INumberBase<T>
    where SELF : QuantityFactory<T, SELF>
{
    static abstract SELF Create(T value);
}

/// <summary>
/// Most general nominally-closed Mass family.
///
/// U, P and T describe the quantity coordinate and numeric carrier.
/// SELF describes the nominal type that arithmetic must preserve.
/// </summary>
public abstract class Mass<U, P, T, SELF> :
    Q<Dimension.Mass, U, P, T>
    where U : Unit<Dimension.Mass>
    where P : Prefix
    where T : INumber<T>
    where SELF : Mass<U, P, T, SELF>, QuantityFactory<T, SELF>
{
    protected Mass(T value) =>
        Value = value;

    public T Value { get; }

    public SELF Add<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>, QuantityFactory<RIGHT_T, RIGHT_SELF> =>
        SELF.Create(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>, QuantityFactory<RIGHT_T, RIGHT_SELF> =>
        SELF.Create(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>, QuantityFactory<RIGHT_T, RIGHT_SELF>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_U.Scale * RIGHT_P.Scale);
        var leftScale = T.CreateChecked(U.Scale * P.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// VSlices recommendation: Grams is the default Mass unit.
/// Prefix, carrier and nominal closure remain free.
/// </summary>
public abstract class Mass<P, T, SELF> : Mass<Grams, P, T, SELF>
    where P : Prefix
    where T : INumber<T>
    where SELF : Mass<P, T, SELF>, QuantityFactory<T, SELF>
{
    protected Mass(T value) : base(value)
    {
    }
}

/// <summary>
/// VSlices recommendation: Kilo is the default prefix over the default Grams unit.
/// Carrier and nominal closure remain free.
/// </summary>
public abstract class Mass<T, SELF> : Mass<Kilo, T, SELF>
    where T : INumber<T>
    where SELF : Mass<T, SELF>, QuantityFactory<T, SELF>
{
    protected Mass(T value) : base(value)
    {
    }
}

/// <summary>
/// Recommended concrete Mass with only the numeric carrier left open.
/// </summary>
public class Mass<T> : Mass<T, Mass<T>>, QuantityFactory<T, Mass<T>>
    where T : INumber<T>
{
    public Mass(T value) : base(value)
    {
    }

    public static Mass<T> Create(T value) =>
        new(value);
}

/// <summary>
/// Fully recommended Mass: Grams + Kilo + double.
/// </summary>
public sealed class Mass : Mass<double, Mass>, QuantityFactory<double, Mass>
{
    public Mass(double value) : base(value)
    {
    }

    public static Mass Create(double value) =>
        new(value);
}

/// <summary>
/// C# 14 extension operators expose the family algebra without moving
/// conversion policy or construction authority into the syntax layer.
/// The result is left-biased in coordinate, carrier and nominal identity.
/// </summary>
public static class MassOperators
{
    extension<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF, RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Mass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>)
        where LEFT_U : Unit<Dimension.Mass>
        where LEFT_P : Prefix
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : Mass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>, QuantityFactory<LEFT_T, LEFT_SELF>
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>, QuantityFactory<RIGHT_T, RIGHT_SELF>
    {
        public static LEFT_SELF operator +(
            Mass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Add(right);

        public static LEFT_SELF operator -(
            Mass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Subtract(right);
    }
}
