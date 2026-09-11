using System.Numerics;

namespace VSlices.Space.Modeling.QuantityFamilies;

/// <summary>
/// Type-level construction authority for a nominal quantity realization.
/// The concrete SELF decides how a backing value becomes that same nominal type.
/// </summary>
public interface QuantityFactory<T, SELF>
    where T : INumberBase<T>
    where SELF : QuantityFactory<T, SELF>
{
    static abstract SELF Create(T value);
}

/// <summary>
/// CRTP pressure probe that preserves nominal closure without instance virtual dispatch.
/// Q owns quantity-family membership; QuantityFactory owns nominal construction authority.
/// </summary>
public abstract class StaticSelfMass<U, P, T, SELF> : Q<Dimension.Mass, U, P, T>
    where U : Unit<Dimension.Mass>
    where P : Prefix
    where T : INumber<T>
    where SELF : StaticSelfMass<U, P, T, SELF>, QuantityFactory<T, SELF>
{
    protected StaticSelfMass(T value) => Value = value;

    public T Value { get; }

    public SELF Add<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>, QuantityFactory<RIGHT_T, RIGHT_SELF> =>
        SELF.Create(Value + ConvertToLeft(right));

    public SELF Subtract<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>, QuantityFactory<RIGHT_T, RIGHT_SELF> =>
        SELF.Create(Value - ConvertToLeft(right));

    private static T ConvertToLeft<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right)
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>, QuantityFactory<RIGHT_T, RIGHT_SELF>
    {
        var rightValue = T.CreateChecked(right.Value);
        var rightScale = T.CreateChecked(RIGHT_U.Scale * RIGHT_P.Scale);
        var leftScale = T.CreateChecked(U.Scale * P.Scale);

        return rightValue * rightScale / leftScale;
    }
}

/// <summary>
/// C# 14 supplies operator syntax while SELF supplies the static nominal result type.
/// Neither operator realization nor Q needs to know how a particular nominal value is constructed.
/// </summary>
public static class StaticSelfMassOperators
{
    extension<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF, RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>(
        StaticSelfMass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>)
        where LEFT_U : Unit<Dimension.Mass>
        where LEFT_P : Prefix
        where LEFT_T : INumber<LEFT_T>
        where LEFT_SELF : StaticSelfMass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF>, QuantityFactory<LEFT_T, LEFT_SELF>
        where RIGHT_U : Unit<Dimension.Mass>
        where RIGHT_P : Prefix
        where RIGHT_T : INumber<RIGHT_T>
        where RIGHT_SELF : StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF>, QuantityFactory<RIGHT_T, RIGHT_SELF>
    {
        public static LEFT_SELF operator +(
            StaticSelfMass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Add(right);

        public static LEFT_SELF operator -(
            StaticSelfMass<LEFT_U, LEFT_P, LEFT_T, LEFT_SELF> left,
            StaticSelfMass<RIGHT_U, RIGHT_P, RIGHT_T, RIGHT_SELF> right) =>
            left.Subtract(right);
    }
}

public abstract class StaticSelfMass<P, T, SELF> : StaticSelfMass<Grams, P, T, SELF>
    where P : Prefix
    where T : INumber<T>
    where SELF : StaticSelfMass<P, T, SELF>, QuantityFactory<T, SELF>
{
    protected StaticSelfMass(T value) : base(value)
    {
    }
}

public abstract class StaticSelfMass<T, SELF> : StaticSelfMass<Kilo, T, SELF>
    where T : INumber<T>
    where SELF : StaticSelfMass<T, SELF>, QuantityFactory<T, SELF>
{
    protected StaticSelfMass(T value) : base(value)
    {
    }
}

public class StaticRecommendedMass<T> :
    StaticSelfMass<T, StaticRecommendedMass<T>>,
    QuantityFactory<T, StaticRecommendedMass<T>>
    where T : INumber<T>
{
    public StaticRecommendedMass(T value) : base(value)
    {
    }

    public static StaticRecommendedMass<T> Create(T value) => new(value);
}

public sealed class StaticNominalMass :
    StaticSelfMass<double, StaticNominalMass>,
    QuantityFactory<double, StaticNominalMass>
{
    public StaticNominalMass(double value) : base(value)
    {
    }

    public static StaticNominalMass Create(double value) => new(value);
}
