using System.Numerics;
using VSlices.Space.Modeling.QuantityFamilies;

namespace VSlices.Space.Modeling.Tests;

public class QuantityFamilyModelingTests
{
    [Fact]
    public void Recommended_mass_is_a_member_of_the_mass_quantity_family()
    {
        var mass = new Mass<double>(2.5d);

        Assert.IsAssignableFrom<Q<Dimension.Mass, Grams, Kilo, double>>(mass);
    }

    [Fact]
    public void Same_coordinate_mass_addition_uses_the_general_family_operator()
    {
        Mass<Grams, Kilo, double> left = new ProbeMass<Grams, Kilo, double>(2d);
        Mass<Grams, Kilo, double> right = new ProbeMass<Grams, Kilo, double>(3d);

        var result = left + right;

        Assert.Equal(5d, result.Value);
    }

    [Fact]
    public void Same_unit_mass_can_add_a_different_prefix_and_backing_type()
    {
        Mass<Grams, Kilo, double> kilograms = new ProbeMass<Grams, Kilo, double>(1d);
        Mass<Grams, Micro, decimal> micrograms = new ProbeMass<Grams, Micro, decimal>(500_000_000m);

        var result = kilograms + micrograms;

        Assert.Equal(1.5d, result.Value, precision: 12);
    }

    [Fact]
    public void Fully_open_mass_can_add_across_unit_prefix_and_backing_type()
    {
        Mass<Grams, None, decimal> grams = new ProbeMass<Grams, None, decimal>(1_000m);
        Mass<Pounds, None, double> pounds = new ProbeMass<Pounds, None, double>(1d);

        var result = grams + pounds;

        Assert.Equal(1_453.59237m, result.Value);
    }

    [Fact]
    public void Fully_open_mass_addition_is_left_biased()
    {
        Mass<Pounds, None, decimal> pounds = new ProbeMass<Pounds, None, decimal>(1m);
        Mass<Grams, Kilo, double> kilograms = new ProbeMass<Grams, Kilo, double>(1d);

        var result = pounds + kilograms;

        var expectedPounds = 1m + (1_000m / Pounds.Scale);
        Assert.Equal(expectedPounds, result.Value);
    }

    [Fact]
    public void Fully_open_mass_subtraction_uses_the_same_left_biased_conversion_policy()
    {
        Mass<Grams, None, decimal> grams = new ProbeMass<Grams, None, decimal>(1_000m);
        Mass<Grams, Kilo, double> kilograms = new ProbeMass<Grams, Kilo, double>(0.25d);

        var result = grams - kilograms;

        Assert.Equal(750m, result.Value);
    }

    [Fact]
    public void Recommended_mass_descendants_can_use_extension_operator_inference()
    {
        var recommended = new Mass<double>(1d);
        Mass<Grams, Micro, decimal> micrograms = new ProbeMass<Grams, Micro, decimal>(500_000_000m);

        var result = recommended + micrograms;

        Assert.Equal(1.5d, result.Value, precision: 12);
    }

    [Fact]
    public void Fully_recommended_mass_can_use_extension_operator_inference()
    {
        var recommended = new vMass(1d);
        Mass<Pounds, None, decimal> pounds = new ProbeMass<Pounds, None, decimal>(1m);

        var result = recommended + pounds;

        Assert.Equal(1d + (double)Pounds.Scale / 1_000d, result.Value, precision: 12);
    }

    private sealed class ProbeMass<U, P, T> : Mass<U, P, T>
        where U : Unit<Dimension.Mass>
        where P : Prefix
        where T : INumber<T>
    {
        public ProbeMass(T value) : base(value)
        {
        }

        protected override Mass<U, P, T> Recreate(T value) =>
            new ProbeMass<U, P, T>(value);
    }
}
