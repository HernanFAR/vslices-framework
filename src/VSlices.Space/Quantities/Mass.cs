using System.Numerics;
using LanguageExt;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Most general nominally closed Mass family.
/// SELF is first because nominal closure is the primary type being defined;
/// C and T describe its coordinate basis and numeric carrier.
/// </summary>
public abstract class Mass<SELF, C, T> : Q<M.Mass, C, T>
    where SELF : Mass<SELF, C, T>
    where C : Coordinate<M.Mass>
    where T : INumber<T>
{
    private static readonly Func<T, SELF> Reconstruct = IL.Ctor<T, SELF>();

    protected Mass(T value) => Value = value;
    public T Value { get; }

    public SELF Add<RIGHT_SELF, RIGHT_C, RIGHT_T>(Mass<RIGHT_SELF, RIGHT_C, RIGHT_T> right)
        where RIGHT_SELF : Mass<RIGHT_SELF, RIGHT_C, RIGHT_T>
        where RIGHT_C : Coordinate<M.Mass>
        where RIGHT_T : INumber<RIGHT_T> =>
        Reconstruct(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_SELF, RIGHT_C, RIGHT_T>(Mass<RIGHT_SELF, RIGHT_C, RIGHT_T> right)
        where RIGHT_SELF : Mass<RIGHT_SELF, RIGHT_C, RIGHT_T>
        where RIGHT_C : Coordinate<M.Mass>
        where RIGHT_T : INumber<RIGHT_T> =>
        Reconstruct(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_SELF, RIGHT_C, RIGHT_T>(Mass<RIGHT_SELF, RIGHT_C, RIGHT_T> right)
        where RIGHT_SELF : Mass<RIGHT_SELF, RIGHT_C, RIGHT_T>
        where RIGHT_C : Coordinate<M.Mass>
        where RIGHT_T : INumber<RIGHT_T>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_C.ReferenceScale);
        var leftScale = T.CreateChecked(C.ReferenceScale);
        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// VSlices recommendation: Kilograms is the default Mass coordinate.
/// The fixed canonical reference used by coordinate scaling remains Gram.
/// </summary>
public abstract class Mass<SELF, T> : Mass<SELF, Kilograms, T>
    where SELF : Mass<SELF, T>
    where T : INumber<T>
{
    protected Mass(T value) : base(value) { }
}

public class Mass<T> : Mass<Mass<T>, T>
    where T : INumber<T>
{
    public Mass(T value) : base(value) { }
}

public sealed class vMass : Mass<vMass, double>
{
    public vMass(double value) : base(value) { }
}

public static class MassOperators
{
    extension<LEFT_SELF, LEFT_C, LEFT_T, RIGHT_SELF, RIGHT_C, RIGHT_T>(Mass<LEFT_SELF, LEFT_C, LEFT_T>)
        where LEFT_SELF : Mass<LEFT_SELF, LEFT_C, LEFT_T>
        where LEFT_C : Coordinate<M.Mass>
        where LEFT_T : INumber<LEFT_T>
        where RIGHT_SELF : Mass<RIGHT_SELF, RIGHT_C, RIGHT_T>
        where RIGHT_C : Coordinate<M.Mass>
        where RIGHT_T : INumber<RIGHT_T>
    {
        public static LEFT_SELF operator +(
            Mass<LEFT_SELF, LEFT_C, LEFT_T> left,
            Mass<RIGHT_SELF, RIGHT_C, RIGHT_T> right) => left.Add(right);

        public static LEFT_SELF operator -(
            Mass<LEFT_SELF, LEFT_C, LEFT_T> left,
            Mass<RIGHT_SELF, RIGHT_C, RIGHT_T> right) => left.Subtract(right);
    }
}
