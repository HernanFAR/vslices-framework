using System.Numerics;
using LanguageExt;

namespace VSlices.Space.Quantities;

/// <summary>
/// Most general nominally closed Length family.
/// U, P and T describe the quantity coordinate and numeric carrier.
/// SELF describes the nominal type that arithmetic preserves.
/// </summary>
public abstract class Length<U, P, T, SELF> : Q<Dimension.Length, U, P, T>
    where U : Unit<Dimension.Length>
    where P : Prefix
    where T : INumber<T>
    where SELF : Length<U, P, T, SELF>
{
    private static readonly Func<T, SELF> Reconstruct = IL.Ctor<T, SELF>();

    protected Length(T value) =>
        Value = value;

    public T Value { get; }

    public SELF Add<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Length>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Length>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Length>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_U.Scale * RIGHT_P.Scale);
        var leftScale = T.CreateChecked(U.Scale * P.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// VSlices recommendation: Meters is the default Length unit.
/// Prefix, carrier and nominal closure remain free.
/// </summary>
public abstract class Length<P, T, SELF> : Length<Meters, P, T, SELF>
    where P : Prefix
    where T : INumber<T>
    where SELF : Length<P, T, SELF>
{
    protected Length(T value) : base(value)
    {
    }
}

/// <summary>
/// VSlices recommendation: no prefix over Meters.
/// Carrier and nominal closure remain free.
/// </summary>
public abstract class Length<T, SELF> : Length<None, T, SELF>
    where T : INumber<T>
    where SELF : Length<T, SELF>
{
    protected Length(T value) : base(value)
    {
    }
}

/// <summary>
/// Recommended Length with only the numeric carrier left open.
/// </summary>
public class Length<T> : Length<T, Length<T>>
    where T : INumber<T>
{
    public Length(T value) : base(value)
    {
    }
}

/// <summary>
/// Fully recommended Length: Meters + no prefix + double.
/// </summary>
public sealed class vLength : Length<double, vLength>
{
    public vLength(double value) : base(value)
    {
    }
}

/// <summary>
/// C# 14 extension operators expose Length algebra while keeping conversion policy in Length.
/// Results are left-biased in Unit, Prefix, carrier, and nominal SELF.
/// </summary>
public static class LengthOperators
{
    extension<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF, RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        Length<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>)
        where LEFT_U : Unit<Dimension.Length>
        where LEFT_P : Prefix
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : Length<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>
        where RIGHT_U : Unit<Dimension.Length>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>
    {
        public static LEFT_SELF operator +(
            Length<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Add(right);

        public static LEFT_SELF operator -(
            Length<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            Length<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Subtract(right);
    }
}
