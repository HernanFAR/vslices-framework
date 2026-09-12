using System.Numerics;
using LanguageExt;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Most general nominally closed Mass family.
/// C and T describe the effective coordinate and numeric carrier.
/// SELF describes the nominal type that arithmetic preserves.
/// </summary>
public abstract class Mass<C, T, SELF> : Q<Dimension.Mass, C, T>
    where C : Coordinate<Dimension.Mass>
    where T : INumber<T>
    where SELF : Mass<C, T, SELF>
{
    private static readonly Func<T, SELF> Reconstruct = IL.Ctor<T, SELF>();

    protected Mass(T value) =>
        Value = value;

    public T Value { get; }

    public SELF Add<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Mass>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_C, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Mass>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_C, RIGHT_T, RIGHT_SELF> =>
        Reconstruct(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Mass<RIGHT_C, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_C : Coordinate<Dimension.Mass>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_C, RIGHT_T, RIGHT_SELF>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_C.Scale);
        var leftScale = T.CreateChecked(C.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// VSlices recommendation: Kilograms is the default Mass coordinate.
/// Carrier and nominal closure remain free.
/// </summary>
public abstract class Mass<T, SELF> : Mass<Kilograms, T, SELF>
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
/// Fully recommended Mass: Kilograms + double.
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
/// Results are left-biased in coordinate, carrier, and nominal SELF.
/// </summary>
public static class MassOperators
{
    extension<LEFT_C, LEFT_T, LEFT_SELF, RIGHT_C, RIGHT_T, RIGHT_SELF>(
        Mass<LEFT_C, LEFT_T, LEFT_SELF>)
        where LEFT_C : Coordinate<Dimension.Mass>
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : Mass<LEFT_C, LEFT_T, LEFT_SELF>
        where RIGHT_C : Coordinate<Dimension.Mass>
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : Mass<RIGHT_C, RIGHT_T, RIGHT_SELF>
    {
        public static LEFT_SELF operator +(
            Mass<LEFT_C, LEFT_T, LEFT_SELF> left,
            Mass<RIGHT_C, RIGHT_T, RIGHT_SELF> right) =>
            left.Add(right);

        public static LEFT_SELF operator -(
            Mass<LEFT_C, LEFT_T, LEFT_SELF> left,
            Mass<RIGHT_C, RIGHT_T, RIGHT_SELF> right) =>
            left.Subtract(right);
    }
}
