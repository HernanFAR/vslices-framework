using System.Numerics;
using VSlices.Space.Modeling.QuantityFamilies;

namespace VSlices.Space.Modeling.Tests;

public class StaticSelfMassModelingTests
{
    [Fact]
    public void Static_factory_preserves_nominal_type_across_cross_family_addition()
    {
        var left = new StaticNominalMass(1d);
        var right = new ProbeStaticMass<Pounds, None, decimal>(1m);

        StaticNominalMass result = left + right;

        Assert.IsType<StaticNominalMass>(result);
        Assert.Equal(1d + (double)Pounds.Scale / 1_000d, result.Value, precision: 12);
    }

    [Fact]
    public void Static_factory_preserves_nominal_type_across_cross_family_subtraction()
    {
        var left = new StaticNominalMass(2d);
        var right = new ProbeStaticMass<Grams, Kilo, decimal>(0.5m);

        StaticNominalMass result = left - right;

        Assert.IsType<StaticNominalMass>(result);
        Assert.Equal(1.5d, result.Value, precision: 12);
    }

    [Fact]
    public void Generic_recommended_mass_keeps_closed_nominal_type_without_virtual_recreate()
    {
        var left = new StaticRecommendedMass<decimal>(1m);
        var right = new ProbeStaticMass<Grams, Micro, double>(500_000_000d);

        StaticRecommendedMass<decimal> result = left + right;

        Assert.IsType<StaticRecommendedMass<decimal>>(result);
        Assert.Equal(1.5m, result.Value);
    }

    [Fact]
    public void Custom_nominal_mass_keeps_Q_membership_and_owns_static_construction()
    {
        var left = new ProbeStaticMass<Grams, None, decimal>(1_000m);
        var right = new ProbeStaticMass<Pounds, None, double>(1d);

        ProbeStaticMass<Grams, None, decimal> result = left + right;

        Assert.IsAssignableFrom<Q<Dimension.Mass, Grams, None, decimal>>(result);
        Assert.Equal(1_453.59237m, result.Value);
    }

    private sealed class ProbeStaticMass<U, P, T> :
        StaticSelfMass<U, P, T, ProbeStaticMass<U, P, T>>,
        QuantityFactory<T, ProbeStaticMass<U, P, T>>
        where U : Unit<Dimension.Mass>
        where P : Prefix
        where T : INumber<T>
    {
        public ProbeStaticMass(T value) : base(value)
        {
        }

        public static ProbeStaticMass<U, P, T> Create(T value) => new(value);
    }
}
