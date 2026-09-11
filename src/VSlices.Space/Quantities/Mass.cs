using System.Numerics;
using LanguageExt;

namespace VSlices.Space.Quantities;

/// <summary>
/// Most general nominally closed Mass family.
/// U, P and T describe the quantity coordinate and numeric carrier.
/// SELF describes the nominal type that arithmetic preserves.
/// </summary>
public abstract class Mass<U, P, T, SELF> : Q<Dimension.Mass, U, P, T>
    where U : Unit<Dimension.Mass>
    where P : Prefix
    where T : INumber<T>
    where SELF : Mass<U, P, T, SELF>
{
    private static readonly Func<T, SELF> Reconstruct = IL.Ctor<T, SELF>();

    protected Mass(T value) =>
        Value = value;

    public T Value { get; }

    public SELF Add<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>
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
    where SELF : Mass<P, T, SELF>
{
    protected Mass(T value) : base(value)
    {
    }
}

/// <summary>
/// VSlices recommendation: Kilo is the default prefix over Grams.
/// Carrier and nominal closure remain free.
/// </summary>
public abstract class Mass<T, SELF> : Mass<Kilo, T, SELF>
    where T : INumber<T>
    where SELF : Mass<T, SELF>
{
    protected Mass(T value) : base(value)
    {
    }
}

/// <summary>
/// Recommended Mass with only the numeric carrier left open.
/// </summary>
public class Mass<T> : Mass<T, Mass<T>>
    where T : INumber<T>
{
    public Mass(T value) : base(value)
    {
    }
}

/// <summary>
/// Fully recommended Mass: Grams + Kilo + double.
/// The v-prefix remains provisional while the Value surface is still being sharpened.
/// </summary>
public sealed class vMass : Mass<double, vMass>
{
    public vMass(double value) : base(value)
    {
    }
}

/// <summary>
/// C# 14 extension operators expose Mass algebra while keeping conversion policy in Mass.
/// Results are left-biased in Unit, Prefix, carrier, and nominal SELF.
/// </summary>
public static class MassOperators
{
    extension<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF, RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Mass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>)
        where LEFT_U : Unit<Dimension.Mass>
        where LEFT_P : Prefix
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : Mass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>
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
