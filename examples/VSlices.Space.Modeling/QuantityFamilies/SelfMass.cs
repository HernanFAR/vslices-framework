using System.Numerics;

namespace VSlices.Space.Modeling.QuantityFamilies;

/// <summary>
/// CRTP pressure probe for preserving the nominal concrete left type across
/// quantity-family arithmetic without moving family membership into SELF.
/// </summary>
public abstract class SelfMass<U, P, T, SELF> : Q<Dimension.Mass, U, P, T>
    where U : Unit<Dimension.Mass>
    where P : Prefix
    where T : INumber<T>
    where SELF : SelfMass<U, P, T, SELF>
{
    protected SelfMass(T value) => Value = value;

    public T Value { get; }

    protected abstract SELF Recreate(T value);

    public SELF Add<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> =>
        Recreate(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> =>
        Recreate(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_U.Scale * RIGHT_P.Scale);
        var leftScale = T.CreateChecked(U.Scale * P.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// C# 14 extension operators infer both operand families including SELF.
/// SELF is deliberately not part of Q: Q says which quantity family a type
/// belongs to; SELF says which nominal type owns re-materialization.
/// </summary>
public static class SelfMassOperators
{
    extension<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF, RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        SelfMass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>)
        where LEFT_U : Unit<Dimension.Mass>
        where LEFT_P : Prefix
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : SelfMass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>
    {
        public static LEFT_SELF operator +(
            SelfMass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Add(right);

        public static LEFT_SELF operator -(
            SelfMass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            SelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Subtract(right);
    }
}

/// <summary>
/// Recommended gram family while preserving the eventual nominal SELF.
/// </summary>
public abstract class SelfMass<P, T, SELF> : SelfMass<Grams, P, T, SELF>
    where P : Prefix
    where T : INumber<T>
    where SELF : SelfMass<P, T, SELF>
{
    protected SelfMass(T value) : base(value)
    {
    }
}

/// <summary>
/// Recommended kilogram family while preserving the eventual nominal SELF.
/// </summary>
public abstract class SelfMass<T, SELF> : SelfMass<Kilo, T, SELF>
    where T : INumber<T>
    where SELF : SelfMass<T, SELF>
{
    protected SelfMass(T value) : base(value)
    {
    }
}

public class RecommendedSelfMass<T> : SelfMass<T, RecommendedSelfMass<T>>
    where T : INumber<T>
{
    public RecommendedSelfMass(T value) : base(value)
    {
    }

    protected override RecommendedSelfMass<T> Recreate(T value) =>
        new(value);
}

public sealed class NominalMass : SelfMass<double, NominalMass>
{
    public NominalMass(double value) : base(value)
    {
    }

    protected override NominalMass Recreate(double value) =>
        new(value);
}
