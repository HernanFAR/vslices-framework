using System.Numerics;

namespace VSlices.Space.Modeling.QuantityFamilies;

public abstract class Dimension
{
    private Dimension()
    {
    }

    public sealed class Mass : Dimension
    {
        private Mass()
        {
        }
    }
}

public interface Unit<F>
    where F : Dimension
{
    static abstract decimal Scale { get; }
}

public readonly struct Grams : Unit<Dimension.Mass>
{
    public static decimal Scale => 1m;
}

public readonly struct Pounds : Unit<Dimension.Mass>
{
    public static decimal Scale => 453.59237m;
}

public interface Prefix
{
    static abstract decimal Scale { get; }
}

public readonly struct None : Prefix
{
    public static decimal Scale => 1m;
}

public readonly struct Kilo : Prefix
{
    public static decimal Scale => 1_000m;
}

public readonly struct Micro : Prefix
{
    public static decimal Scale => 0.000001m;
}

/// <summary>
/// Quantity-family membership.
/// F identifies the dimensional family, U the unit, P the prefix, and T the backing numeric type.
/// </summary>
public interface Q<F, U, P, T>
    where F : Dimension
    where U : Unit<F>
    where P : Prefix
    where T : INumberBase<T>
{
    T Value { get; }
}

/// <summary>
/// Most general Mass family. Unit, prefix, and backing type remain consumer-owned choices.
/// </summary>
public abstract class Mass<U, P, T> : Q<Dimension.Mass, U, P, T>
    where U : Unit<Dimension.Mass>
    where P : Prefix
    where T : INumber<T>
{
    protected Mass(T value) =>
        Value = value;

    public T Value { get; }

    protected abstract Mass<U, P, T> Recreate(T value);

    public static Mass<U, P, T> operator +(
        Mass<U, P, T> left,
        Mass<U, P, T> right) =>
        left.Recreate(left.Value + right.Value);

    public static Mass<U, P, T> operator -(
        Mass<U, P, T> left,
        Mass<U, P, T> right) =>
        left.Recreate(left.Value - right.Value);

    public Mass<U, P, T> Add<RIGHT_U, RIGHT_P, RIGHT_T>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T> =>
        Recreate(Value + ConvertToLeft(right));

    public Mass<U, P, T> Subtract<RIGHT_U, RIGHT_P, RIGHT_T>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T> =>
        Recreate(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_U, RIGHT_P, RIGHT_T>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_U.Scale * RIGHT_P.Scale);
        var leftScale = T.CreateChecked(U.Scale * P.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// C# 14 extension operators allow both operand families to contribute inferred generic parameters.
/// The semantic operation remains owned by Mass; the extension block only supplies operator realization.
/// </summary>
public static class MassOperators
{
    extension<LEFT_U, LEFT_P, LEFT_T, RIGHT_U, RIGHT_P, RIGHT_T>(Mass<LEFT_U, LEFT_P, LEFT_T>)
        where LEFT_U : Unit<Dimension.Mass>
        where LEFT_P : Prefix
        where LEFT_T : INumber<LEFT_T>
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
    {
        public static Mass<LEFT_U, LEFT_P, LEFT_T> operator +(
            Mass<LEFT_U, LEFT_P, LEFT_T> left,
            Mass<RIGHT_U, RIGHT_P, RIGHT_T> right) =>
            left.Add(right);

        public static Mass<LEFT_U, LEFT_P, LEFT_T> operator -(
            Mass<LEFT_U, LEFT_P, LEFT_T> left,
            Mass<RIGHT_U, RIGHT_P, RIGHT_T> right) =>
            left.Subtract(right);
    }
}

public abstract class Mass<P, T> : Mass<Grams, P, T>
    where P : Prefix
    where T : INumber<T>
{
    protected Mass(T value) : base(value)
    {
    }
}

public class Mass<T> : Mass<Kilo, T>
    where T : INumber<T>
{
    public Mass(T value) : base(value)
    {
    }

    protected override Mass<Grams, Kilo, T> Recreate(T value) =>
        new Mass<T>(value);
}

public sealed class vMass : Mass<double>
{
    public vMass(double value) : base(value)
    {
    }

    protected override Mass<Grams, Kilo, double> Recreate(double value) =>
        new vMass(value);
}
