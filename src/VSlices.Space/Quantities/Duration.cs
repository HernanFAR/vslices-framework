using System.Numerics;
using LanguageExt;

namespace VSlices.Space.Quantities;

/// <summary>
/// Most general nominally closed Duration family.
/// C and T describe the effective coordinate and numeric carrier.
/// SELF describes the nominal type that arithmetic preserves.
/// </summary>
public abstract class Duration<C, T, SELF> : Q<Dimension.Duration, C, T>
    where C : Coordinate<Dimension.Duration>
    where T : INumber<T>
    where SELF : Duration<C, T, SELF>
{
    private static readonly Func<T, SELF> Reconstruct = IL.Ctor<T, SELF>();

    protected Duration(T value) =>
        Value = value;

    public T Value { get; }

    public SELF Add<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Duration<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Duration>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Duration<RIGHT_C, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Duration<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Duration>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Duration<RIGHT_C, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Duration<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Duration>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Duration<RIGHT_C, RIGHT_T, RIGHT_SELF>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_C.Scale);
        var leftScale = T.CreateChecked(C.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// VSlices recommendation: Seconds is the default Duration coordinate.
/// Carrier and nominal closure remain free.
/// </summary>
public abstract class Duration<T, SELF> : Duration<Seconds, T, SELF>
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
/// Fully recommended Duration: Seconds + double.
/// </summary>
public sealed class vDuration : Duration<double, vDuration>
{
    public vDuration(double value) : base(value)
    {
    }
}

/// <summary>
/// C# 14 extension operators expose Duration algebra while keeping conversion policy in Duration.
/// Results are left-biased in coordinate, carrier, and nominal SELF.
/// </summary>
public static class DurationOperators
{
    extension<LEFT_C, LEFT_T, LEFT_SELF, RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Duration<LEFT_C, LEFT_T, LEFT_SELF>)
        where LEFT_C : Coordinate<Dimension.Duration>
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : Duration<LEFT_C, LEFT_T, LEFT_SELF>
        where RIGHT_C : Coordinate<Dimension.Duration>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Duration<RIGHT_C, RIGHT_T, RIGHT_SELF>
    {
        public static LEFT_SELF operator +(
            Duration<LEFT_C, LEFT_T, LEFT_SELF> left,
            Duration<RIGHT_C, RIGHT_T, RIGHT_SELF> right) =>
            left.Add(right);

        public static LEFT_SELF operator -(
            Duration<LEFT_C, LEFT_T, LEFT_SELF> left,
            Duration<RIGHT_C, RIGHT_T, RIGHT_SELF> right) =>
            left.Subtract(right);
    }
}
