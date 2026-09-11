using System.Numerics;
using VSlices.Space.Quantities;

namespace VSlices.Space.Tests;

public class LengthDurationTests
{
    [Fact]
    public void vLength_is_the_fully_recommended_length_family_member()
    {
        var length = new vLength(2.5d);

        Assert.IsAssignableFrom<Q<Dimension.Length, Meters, None, double>>(length);
        Assert.Equal(2.5d, length.Value);
    }

    [Fact]
    public void vLength_preserves_nominal_type_across_unit_prefix_and_carrier_conversion()
    {
        var meters = new vLength(1d);
        var kilometers = new ProbeLength<Meters, Kilo, decimal>(1m);
        var feet = new ProbeLength<Feet, None, decimal>(1m);

        vLength result = meters + kilometers + feet;

        Assert.IsType<vLength>(result);
        Assert.Equal(1001.3048d, result.Value, precision: 10);
    }

    [Fact]
    public void generic_length_preserves_its_closed_nominal_type()
    {
        var meters = new Length<decimal>(10m);
        var microMeters = new ProbeLength<Meters, Micro, double>(500_000d);

        Length<decimal> result = meters + microMeters;

        Assert.IsType<Length<decimal>>(result);
        Assert.Equal(10.5m, result.Value);
    }

    [Fact]
    public void vDuration_is_the_fully_recommended_duration_family_member()
    {
        var duration = new vDuration(2.5d);

        Assert.IsAssignableFrom<Q<Dimension.Duration, Seconds, None, double>>(duration);
        Assert.Equal(2.5d, duration.Value);
    }

    [Fact]
    public void vDuration_preserves_nominal_type_across_unit_prefix_and_carrier_conversion()
    {
        var seconds = new vDuration(30d);
        var minutes = new ProbeDuration<Minutes, None, decimal>(1m);
        var microSeconds = new ProbeDuration<Seconds, Micro, decimal>(500_000m);

        vDuration result = seconds + minutes + microSeconds;

        Assert.IsType<vDuration>(result);
        Assert.Equal(90.5d, result.Value, precision: 10);
    }

    [Fact]
    public void duration_subtraction_preserves_the_left_coordinate_and_nominal_type()
    {
        var minutes = new ProbeDuration<Minutes, None, decimal>(2m);
        var seconds = new vDuration(30d);

        ProbeDuration<Minutes, None, decimal> result = minutes - seconds;

        Assert.IsType<ProbeDuration<Minutes, None, decimal>>(result);
        Assert.Equal(1.5m, result.Value);
    }

    private sealed class ProbeLength<U, P, T> : Length<U, P, T, ProbeLength<U, P, T>>
        where U : Unit<Dimension.Length>
        where P : Prefix
        where T : INumber<T>
    {
        public ProbeLength(T value) : base(value)
        {
        }
    }

    private sealed class ProbeDuration<U, P, T> : Duration<U, P, T, ProbeDuration<U, P, T>>
        where U : Unit<Dimension.Duration>
        where P : Prefix
        where T : INumber<T>
    {
        public ProbeDuration(T value) : base(value)
        {
        }
    }
}
