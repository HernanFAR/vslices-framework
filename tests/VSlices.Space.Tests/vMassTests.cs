using System.Numerics;
using VSlices.Space.Quantities;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Tests;

public class vMassTests
{
    [Fact]
    public void vMass_is_the_fully_recommended_mass_family_member()
    {
        var mass = new vMass(2.5d);

        Assert.IsAssignableFrom<Q<M.Mass, Kilograms, double>>(mass);
        Assert.Equal(2.5d, mass.Value);
    }

    [Fact]
    public void reference_scale_orientation_is_explicit()
    {
        Assert.Equal(1m, Grams.ReferenceScale);
        Assert.Equal(1_000m, Kilograms.ReferenceScale);
        Assert.Equal(453.59237m, Pounds.ReferenceScale);
    }

    [Fact]
    public void vMass_preserves_its_nominal_type_across_cross_coordinate_addition()
    {
        var left = new vMass(1d);
        var right = new ProbeMass<Pounds, decimal>(1m);

        vMass result = left + right;

        Assert.IsType<vMass>(result);
        Assert.Equal(
            1d + (double)(Pounds.ReferenceScale / Kilograms.ReferenceScale),
            result.Value,
            precision: 12);
    }

    [Fact]
    public void vMass_adds_different_effective_coordinates_and_carriers()
    {
        var kilograms = new vMass(1d);
        var micrograms = new ProbeMass<Micrograms, decimal>(500_000_000m);

        vMass result = kilograms + micrograms;

        Assert.Equal(1.5d, result.Value, precision: 12);
    }

    [Fact]
    public void subtraction_uses_the_same_left_biased_policy()
    {
        var grams = new ProbeMass<Grams, decimal>(1_000m);
        var kilograms = new vMass(0.25d);

        ProbeMass<Grams, decimal> result = grams - kilograms;

        Assert.IsType<ProbeMass<Grams, decimal>>(result);
        Assert.Equal(750m, result.Value);
    }

    [Fact]
    public void recommended_generic_mass_preserves_its_closed_nominal_type()
    {
        var left = new Mass<decimal>(1m);
        var right = new ProbeMass<Micrograms, double>(500_000_000d);

        Mass<decimal> result = left + right;

        Assert.IsType<Mass<decimal>>(result);
        Assert.Equal(1.5m, result.Value);
    }

    private sealed class ProbeMass<C, T> : Mass<ProbeMass<C, T>, C, T>
        where C : Coordinate<M.Mass>
        where T : INumber<T>
    {
        public ProbeMass(T value) : base(value) { }
    }
}
