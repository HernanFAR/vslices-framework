using System.Numerics;
using VSlices.Space.Quantities;
using VSlices.Space.Quantities.Abstract;
using static VSlices.Space.Conversions;

namespace VSlices.Space.Tests;

public class QuotientTests
{
    [Fact]
    public void length_divided_by_duration_preserves_both_primitive_coordinate_bases()
    {
        var distance = new ProbeLength<Kilometers, decimal>(120m);
        var duration = new ProbeDuration<Hours, decimal>(2m);

        Quotient<
            M.Length,
            Kilometers,
            M.Duration,
            Hours,
            decimal> quotient = distance / duration;

        Assert.Equal(60m, quotient.Value);
    }

    [Fact]
    public void structural_quotient_does_not_claim_one_coordinate_q_shape()
    {
        var quotientType = typeof(Quotient<
            M.Length,
            Kilometers,
            M.Duration,
            Hours,
            decimal>);

        Assert.DoesNotContain(
            quotientType.GetInterfaces(),
            candidate => candidate.IsGenericType &&
                         candidate.GetGenericTypeDefinition() == typeof(Q<,,>));
    }

    [Fact]
    public void speed_is_explicitly_established_from_length_per_duration()
    {
        var distance = new ProbeLength<Kilometers, decimal>(120m);
        var duration = new ProbeDuration<Hours, decimal>(2m);
        var structural = distance / duration;

        Speed<Kilometers, Hours, decimal> semantic = speed(structural);
        Quotient<M.Length, Kilometers, M.Duration, Hours, decimal> widened =
            WidenSpeed(semantic);

        Assert.Equal(60m, semantic.Value);
        Assert.Equal(structural, widened);
    }

    [Fact]
    public void speed_preserves_two_coordinate_bases_instead_of_forcing_q()
    {
        var speedType = typeof(Speed<Kilometers, Hours, decimal>);

        Assert.DoesNotContain(
            speedType.GetInterfaces(),
            candidate => candidate.IsGenericType &&
                         candidate.GetGenericTypeDefinition() == typeof(Q<,,>));

        var attribute = Assert.Single(
            speedType
                .GetCustomAttributes(typeof(AlgebraicSymbolAttribute), inherit: false)
                .Cast<AlgebraicSymbolAttribute>());

        Assert.Equal("speed", attribute.Symbol);
    }

    [Fact]
    public void quotient_keeps_coordinate_choice_semantic_instead_of_silently_normalizing()
    {
        var distance = new ProbeLength<Kilometers, decimal>(120m);
        var hours = new ProbeDuration<Hours, decimal>(2m);
        var minutes = new ProbeDuration<Minutes, decimal>(120m);

        var kilometersPerHour = distance / hours;
        var kilometersPerMinute = distance / minutes;

        Assert.Equal(60m, kilometersPerHour.Value);
        Assert.Equal(1m, kilometersPerMinute.Value);
    }

    private static Quotient<M.Length, LENGTH_C, M.Duration, DURATION_C, T>
        WidenSpeed<LENGTH_C, DURATION_C, T>(Speed<LENGTH_C, DURATION_C, T> speed)
        where LENGTH_C : Coordinate<M.Length>
        where DURATION_C : Coordinate<M.Duration>
        where T : INumber<T> =>
        speed.ToBase();

    private sealed class ProbeLength<C, T> : Length<ProbeLength<C, T>, C, T>
        where C : Coordinate<M.Length>
        where T : INumber<T>
    {
        public ProbeLength(T value) : base(value) { }
    }

    private sealed class ProbeDuration<C, T> : Duration<ProbeDuration<C, T>, C, T>
        where C : Coordinate<M.Duration>
        where T : INumber<T>
    {
        public ProbeDuration(T value) : base(value) { }
    }
}
