using System.Numerics;
using LanguageExt;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Most general nominally closed Length family.
/// SELF is first because nominal closure is the primary type being defined;
/// C and T describe its coordinate basis and numeric carrier.
/// </summary>
public abstract class Length<SELF, C, T> : Q<M.Length, C, T>
    where SELF : Length<SELF, C, T>
    where C : Coordinate<M.Length>
    where T : INumber<T>
{
    private static readonly Func<T, SELF> Reconstruct = IL.Ctor<T, SELF>();

    protected Length(T value) => Value = value;
    public T Value { get; }

    public SELF Add<RIGHT_SELF, RIGHT_C, RIGHT_T>(Length<RIGHT_SELF, RIGHT_C, RIGHT_T> right)
        where RIGHT_SELF : Length<RIGHT_SELF, RIGHT_C, RIGHT_T>
        where RIGHT_C : Coordinate<M.Length>
        where RIGHT_T : INumber<RIGHT_T> =>
        Reconstruct(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_SELF, RIGHT_C, RIGHT_T>(Length<RIGHT_SELF, RIGHT_C, RIGHT_T> right)
        where RIGHT_SELF : Length<RIGHT_SELF, RIGHT_C, RIGHT_T>
        where RIGHT_C : Coordinate<M.Length>
        where RIGHT_T : INumber<RIGHT_T> =>
        Reconstruct(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_SELF, RIGHT_C, RIGHT_T>(Length<RIGHT_SELF, RIGHT_C, RIGHT_T> right)
        where RIGHT_SELF : Length<RIGHT_SELF, RIGHT_C, RIGHT_T>
        where RIGHT_C : Coordinate<M.Length>
        where RIGHT_T : INumber<RIGHT_T>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_C.ReferenceScale);
        var leftScale = T.CreateChecked(C.ReferenceScale);
        return rightValue * rightScale / leftScale;
    }
}

public abstract class Length<SELF, T> : Length<SELF, Meters, T>
    where SELF : Length<SELF, T>
    where T : INumber<T>
{
    protected Length(T value) : base(value) { }
}

public class Length<T> : Length<Length<T>, T>
    where T : INumber<T>
{
    public Length(T value) : base(value) { }
}

public sealed class vLength : Length<vLength, double>
{
    public vLength(double value) : base(value) { }
}

public static class LengthOperators
{
    extension<LEFT_SELF, LEFT_C, LEFT_T, RIGHT_SELF, RIGHT_C, RIGHT_T>(Length<LEFT_SELF, LEFT_C, LEFT_T>)
        where LEFT_SELF : Length<LEFT_SELF, LEFT_C, LEFT_T>
        where LEFT_C : Coordinate<M.Length>
        where LEFT_T : INumber<LEFT_T>
        where RIGHT_SELF : Length<RIGHT_SELF, RIGHT_C, RIGHT_T>
        where RIGHT_C : Coordinate<M.Length>
        where RIGHT_T : INumber<RIGHT_T>
    {
        public static LEFT_SELF operator +(
            Length<LEFT_SELF, LEFT_C, LEFT_T> left,
            Length<RIGHT_SELF, RIGHT_C, RIGHT_T> right) => left.Add(right);

        public static LEFT_SELF operator -(
            Length<LEFT_SELF, LEFT_C, LEFT_T> left,
            Length<RIGHT_SELF, RIGHT_C, RIGHT_T> right) => left.Subtract(right);
    }
}

public static class LengthProductOperators
{
    extension<LEFT_SELF, LEFT_C, RIGHT_SELF, RIGHT_C, T>(Length<LEFT_SELF, LEFT_C, T>)
        where LEFT_SELF : Length<LEFT_SELF, LEFT_C, T>
        where LEFT_C : Coordinate<M.Length>
        where RIGHT_SELF : Length<RIGHT_SELF, RIGHT_C, T>
        where RIGHT_C : Coordinate<M.Length>
        where T : INumber<T>
    {
        public static Product<M.Length, M.Length, LEFT_C, T> operator *(
            Length<LEFT_SELF, LEFT_C, T> left,
            Length<RIGHT_SELF, RIGHT_C, T> right)
        {
            var rightValue = T.CreateChecked(right.Value);
            var rightScale = T.CreateChecked(RIGHT_C.ReferenceScale);
            var leftScale = T.CreateChecked(LEFT_C.ReferenceScale);
            var rightInLeftCoordinate = rightValue * rightScale / leftScale;

            return new(left.Value * rightInLeftCoordinate);
        }
    }
}
