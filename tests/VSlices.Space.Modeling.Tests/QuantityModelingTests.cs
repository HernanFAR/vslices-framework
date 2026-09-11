using LanguageExt;
using VSlices.Space.Modeling.Quantities;
using VSlices.Space.Traits;

namespace VSlices.Space.Modeling.Tests;

public class QuantityModelingTests
{
    [Fact]
    public void Prefix_conversion_preserves_the_same_dimensional_quantity()
    {
        var kilo = new Quantity<MassDimension, Kilo>(1d);
        var micro = kilo.In<Micro>();
        var backToKilo = micro.In<Kilo>();

        Assert.Equal(1_000_000_000d, micro.CanonValue);
        Assert.Equal(kilo.CanonValue, backToKilo.CanonValue);
    }

    [Fact]
    public void Mass_is_established_from_its_canonical_structural_quantity()
    {
        var result = Transformable.Transform<Quantity<MassDimension, Kilo>, Mass>(
            new Quantity<MassDimension, Kilo>(2.5d));

        var mass = Success(result);

        Assert.Equal(2.5d, mass.CanonValue);
    }

    [Fact]
    public void Mass_vector_operations_are_closed_over_mass()
    {
        var left = Mass(7d);
        var right = Mass(2d);

        Assert.Equal(9d, (left + right).CanonValue);
        Assert.Equal(5d, (left - right).CanonValue);
        Assert.Equal(-7d, (-left).CanonValue);
        Assert.Equal(14d, (left * 2d).CanonValue);
        Assert.Equal(3.5d, (left / 2d).CanonValue);
        Assert.Equal(3.5d, left / right);
    }

    [Fact]
    public void Mass_subtraction_may_produce_a_negative_mass_quantity()
    {
        var smaller = Mass(2d);
        var larger = Mass(5d);

        var difference = smaller - larger;

        Assert.Equal(-3d, difference.CanonValue);
    }

    [Fact]
    public void Mass_can_expose_an_alternate_prefix_without_changing_its_canonical_metric()
    {
        var mass = Mass(1d);

        var micro = mass.In<Micro>();

        Assert.Equal(1_000_000_000d, micro.CanonValue);
        Assert.Equal(1d, mass.CanonValue);
    }

    private static Mass Mass(double canonicalValue) =>
        Success(Transformable.Transform<Quantity<MassDimension, Kilo>, Mass>(
            new Quantity<MassDimension, Kilo>(canonicalValue)));

    private static T Success<T>(Fin<T> result) =>
        result.Match(
            Succ: value => value,
            Fail: error => throw new Xunit.Sdk.XunitException(
                $"Expected success, but got: {error}"));
}
