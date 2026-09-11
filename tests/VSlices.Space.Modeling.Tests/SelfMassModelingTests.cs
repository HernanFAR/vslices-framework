using System.Numerics;
using VSlices.Space.Modeling.QuantityFamilies;

namespace VSlices.Space.Modeling.Tests;

public class SelfMassModelingTests
{
    [Fact]
    public void Fully_recommended_mass_preserves_its_nominal_static_type_across_cross_family_addition()
    {
        var left = new NominalMass(1d);
        var right = new ProbeSelfMass<Pounds, None, decimal>(1m);

        NominalMass result = left + right;

        Assert.IsType<NominalMass>(result);
        Assert.Equal(1d + (double)Pounds.Scale / 1_000d, result.Value, precision: 12);
    }

    [Fact]
    public void Fully_recommended_mass_preserves_its_nominal_static_type_across_cross_family_subtraction()
    {
        var left = new NominalMass(2d);
        var right = new ProbeSelfMass<Grams, Kilo, decimal>(0.5m);

        NominalMass result = left - right;

        Assert.IsType<NominalMass>(result);
        Assert.Equal(1.5d, result.Value, precision: 12);
    }

    [Fact]
    public void Generic_recommended_mass_preserves_its_closed_nominal_type()
    {
        var left = new RecommendedSelfMass<decimal>(1m);
        var right = new ProbeSelfMass<Grams, Micro, double>(500_000_000d);

        RecommendedSelfMass<decimal> result = left + right;

        Assert.IsType<RecommendedSelfMass<decimal>>(result);
        Assert.Equal(1.5m, result.Value);
    }

    [Fact]
    public void Custom_nominal_mass_can_keep_its_own_type_without_changing_Q_membership()
    {
        var left = new ProbeSelfMass<Grams, None, decimal>(1_000m);
        var right = new ProbeSelfMass<Pounds, None, double>(1d);

        ProbeSelfMass<Grams, None, decimal> result = left + right;

        Assert.IsAssignableFrom<Q<Dimension.Mass, Grams, None, decimal>>(result);
        Assert.Equal(1_453.59237m, result.Value);
    }

    private sealed class ProbeSelfMass<U, P, T> : SelfMass<U, P, T, ProbeSelfMass<U, P, T>>
        where U : Unit<Dimension.Mass>
        where P : Prefix
        where T : INumber<T>
    {
        public ProbeSelfMass(T value) : base(value)
        {
        }

        protected override ProbeSelfMass<U, P, T> Recreate(T value) =>
            new(value);
    }
}
