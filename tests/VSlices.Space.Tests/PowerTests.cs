using VSlices.Space.Quantities;
using VSlices.Space.Quantities.Abstract;
using static VSlices.Space.Quantities.PowerOperations;

namespace VSlices.Space.Tests;

public class PowerTests
{
    [Fact]
    public void square_materializes_length_power_over_the_same_primitive_basis()
    {
        var side = new ProbeLength<Kilometers, decimal>(3m);

        Power<M.Length, N2, Kilometers, decimal> structural = square(side);

        Assert.Equal(9m, structural.Value);
        Assert.IsAssignableFrom<Q<M.Pow<M.Length, N2>, Kilometers, decimal>>(structural);
    }

    [Fact]
    public void square_preserves_the_coordinate_basis_without_creating_a_squared_coordinate()
    {
        var side = new ProbeLength<Meters, double>(4d);

        var structural = square(side);

        Assert.IsType<Power<M.Length, N2, Meters, double>>(structural);
        Assert.Equal(16d, structural.Value);
        Assert.Equal(1m, Meters.ReferenceScale);
    }

    [Fact]
    public void pow_and_mul_shapes_remain_structurally_distinct_until_equivalence_is_defined()
    {
        var side = new ProbeLength<Kilometers, decimal>(3m);

        var powered = square(side);
        var multiplied = side * side;

        Assert.Equal(multiplied.Value, powered.Value);
        Assert.IsAssignableFrom<Q<M.Pow<M.Length, N2>, Kilometers, decimal>>(powered);
        Assert.IsAssignableFrom<Q<M.Mul<M.Length, M.Length>, Kilometers, decimal>>(multiplied);
    }

    private sealed class ProbeLength<C, T> : Length<ProbeLength<C, T>, C, T>
        where C : Coordinate<M.Length>
        where T : System.Numerics.INumber<T>
    {
        public ProbeLength(T value) : base(value) { }
    }
}
