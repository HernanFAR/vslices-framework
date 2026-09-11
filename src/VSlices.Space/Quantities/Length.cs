using System.Numerics;
using LanguageExt;

namespace VSlices.Space.Quantities;

/// <summary>
/// Most general nominally closed Length family.
/// C and T describe the effective coordinate and numeric carrier.
/// SELF describes the nominal type that arithmetic preserves.
/// </summary>
public abstract class Length<C, T, SELF> : Q<Dimension.Length, C, T>
    where C : Coordinate<Dimension.Length>
    where T : INumber<T>
    where SELF : Length<C, T, SELF>
{
    private static readonly Func<T, SELF> Reconstruct = IL.Ctor<T, SELF>();

    protected Length(T value) =>
        Value = value;

    public T Value { get; }

    public SELF Add<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Length<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Length>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Length<RIGHT_C, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Length<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Length>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Length<RIGHT_C, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Length<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Length>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Length<RIGHT_C, RIGHT_T, RIGHT_SELF>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_C.Scale);
        var leftScale = T.CreateChecked(C.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// VSlices recommendation: Meters is the default Length coordinate.
/// Carrier and nominal closure remain free.
/// </summary>
public abstract class Length<T, SELF> : Length<Meters, T, SELF>
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
/// Fully recommended Length: Meters + double.
/// </summary>
public sealed class vLength : Length<double, vLength>
{
    public vLength(double value) : base(value)
    {
    }
}

/// <summary>
/// C# 14 extension operators expose Length algebra while keeping conversion policy in Length.
/// Results are left-biased in coordinate, carrier, and nominal SELF.
/// </summary>
public static class LengthOperators
{
    extension<LEFT_C, LEFT_T, LEFT_SELF, RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Length<LEFT_C, LEFT_T, LEFT_SELF>)
        where LEFT_C : Coordinate<Dimension.Length>
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : Length<LEFT_C, LEFT_T, LEFT_SELF>
        where RIGHT_C : Coordinate<Dimension.Length>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Length<RIGHT_C, RIGHT_T, RIGHT_SELF>
    {
        public static LEFT_SELF operator +(
            Length<LEFT_C, LEFT_T, LEFT_SELF> left,
            Length<RIGHT_C, RIGHT_T, RIGHT_SELF> right) =>
            left.Add(right);

        public static LEFT_SELF operator -(
            Length<LEFT_C, LEFT_T, LEFT_SELF> left,
            Length<RIGHT_C, RIGHT_T, RIGHT_SELF> right) =>
            left.Subtract(right);
    }
}

/// <summary>
/// First production Product pressure. Multiplication is intentionally declared
/// only for Length x Length and requires both operands to share the same carrier.
/// Product availability is therefore explicit rather than universal over Q.
/// </summary>
public static class LengthProductOperators
{
    extension<LEFT_C, RIGHT_C, T, LEFT_SELF, RIGHT_SELF>(
        Length<LEFT_C, T, LEFT_SELF>)
        where LEFT_C : Coordinate<Dimension.Length>
        where RIGHT_C : Coordinate<Dimension.Length>
        where T : INumber<T>
        where LEFT_SELF : Length<LEFT_C, T, LEFT_SELF>
        where RIGHT_SELF : Length<RIGHT_C, T, RIGHT_SELF>
    {
        public static Product<
            Dimension.Length,
            LEFT_C,
            Dimension.Length,
            RIGHT_C,
            T> operator *(
                Length<LEFT_C, T, LEFT_SELF> left,
                Length<RIGHT_C, T, RIGHT_SELF> right) =>
            new(left.Value * right.Value);
    }
}
