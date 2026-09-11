using System.Numerics;
using VSlices.Space.Modeling.QuantityFamilies;
using VSlices.Space.Modeling.QuantityFamilies.Formalization;
using FormalMass = VSlices.Space.Modeling.QuantityFamilies.Formalization.Mass;

namespace VSlices.Space.Modeling.Tests;

public class FormalizedMassModelingTests
{
    [Fact]
    public void Fully_recommended_mass_is_the_default_mass_family_member()
    {
        var mass = new FormalMass(2.5d);

        Assert.IsAssignableFrom<Q<Dimension.Mass, Grams, Kilo, double>>(mass);
    }

    [Fact]
    public void Fully_recommended_mass_preserves_its_nominal_type_across_cross_family_addition()
    {
        var left = new FormalMass(1d);
        var right = new ProbeMass<Pounds, None, decimal>(1m);

        FormalMass result = left + right;

        Assert.IsType<FormalMass>(result);
        Assert.Equal(1d + (double)Pounds.Scale / 1_000d, result.Value, precision: 12);
    }

    [Fact]
    public void Generic_recommended_mass_preserves_its_closed_nominal_type()
    {
        var left = new VSlices.Space.Modeling.QuantityFamilies.Formalization.Mass<decimal>(1m);
        var right = new ProbeMass<Grams, Micro, double>(500_000_000d);

        VSlices.Space.Modeling.QuantityFamilies.Formalization.Mass<decimal> result = left + right;

        Assert.IsType<VSlices.Space.Modeling.QuantityFamilies.Formalization.Mass<decimal>>(result);
        Assert.Equal(1.5m, result.Value);
    }

    [Fact]
    public void Consumer_can_define_a_custom_nominal_mass_at_the_fully_open_family_level()
    {
        var left = new ProbeMass<Grams, None, decimal>(1_000m);
        var right = new ProbeMass<Pounds, None, double>(1d);

        ProbeMass<Grams, None, decimal> result = left + right;

        Assert.IsAssignableFrom<Q<Dimension.Mass, Grams, None, decimal>>(result);
        Assert.Equal(1_453.59237m, result.Value);
    }

    [Fact]
    public void Subtraction_uses_the_same_left_biased_coordinate_and_nominal_policy()
    {
        var left = new ProbeMass<Grams, None, decimal>(1_000m);
        var right = new VSlices.Space.Modeling.QuantityFamilies.Formalization.Mass<double>(0.25d);

        ProbeMass<Grams, None, decimal> result = left - right;

        Assert.Equal(750m, result.Value);
    }

    private sealed class ProbeMass<U, P, T> :
        VSlices.Space.Modeling.QuantityFamilies.Formalization.Mass<U, P, T, ProbeMass<U, P, T>>,
        QuantityFactory<T, ProbeMass<U, P, T>>
        where U : Unit<Dimension.Mass>
        where P : Prefix
        where T : INumber<T>
    {
        public ProbeMass(T value) : base(value)
        {
        }

        public static ProbeMass<U, P, T> Create(T value) =>
            new(value);
    }
}
