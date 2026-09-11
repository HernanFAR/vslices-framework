using System.Numerics;
using LanguageExt;

namespace VSlices.Space.Quantities;

/// <summary>
/// Most general nominally closed Duration family.
/// U, P and T describe the quantity coordinate and numeric carrier.
/// SELF describes the nominal type that arithmetic preserves.
/// </summary>
public abstract class Duration<U, P, T, SELF> : Q<Dimension.Duration, U, P, T>
    where U : Unit<Dimension.Duration>
    where P : Prefix
    where T : INumber<T>
    where SELF : Duration<U, P, T, SELF>
{
    private static readonly Func<T, SELF> Reconstruct = IL.Ctor<T, SELF>();

    protected Duration(T value) =>
        Value = value;

    public T Value { get; }

    public SELF Add<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Duration>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Duration>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Duration>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_U.Scale * RIGHT_P.Scale);
        var leftScale = T.CreateChecked(U.Scale * P.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// VSlices recommendation: Seconds is the default Duration unit.
/// Prefix, carrier and nominal closure remain free.
/// </summary>
public abstract class Duration<P, T, SELF> : Duration<Seconds, P, T, SELF>
    where P : Prefix
    where T : INumber<T>
    where SELF : Duration<P, T, SELF>
{
    protected Duration(T value) : base(value)
    {
    }
}

/// <summary>
/// VSlices recommendation: no prefix over Seconds.
/// Carrier and nominal closure remain free.
/// </summary>
public abstract class Duration<T, SELF> : Duration<None, T, SELF>
    where T : INumber<T>
    where SELF : Duration<T, SELF>
{
    protected Duration(T value) : base(value)
    {
    }
}

/// <summary>
/// Recommended Duration with only the numeric carrier left open.
/// </summary>
public class Duration<T> : Duration<T, Duration<T>>
    where T : INumber<T>
{
    public Duration(T value) : base(value)
    {
    }
}

/// <summary>
/// Fully recommended Duration: Seconds + no prefix + double.
/// </summary>
public sealed class vDuration : Duration<double, vDuration>
{
    public vDuration(double value) : base(value)
    {
    }
}

/// <summary>
/// C# 14 extension operators expose Duration algebra while keeping conversion policy in Duration.
/// Results are left-biased in Unit, Prefix, carrier, and nominal SELF.
/// </summary>
public static class DurationOperators
{
    extension<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF, RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Duration<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>)
        where LEFT_U : Unit<Dimension.Duration>
        where LEFT_P : Prefix
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : Duration<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>
        where RIGHT_U : Unit<Dimension.Duration>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>
    {
        public static LEFT_SELF operator +(
            Duration<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Add(right);

        public static LEFT_SELF operator -(
            Duration<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            Duration<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Subtract(right);
    }
}
