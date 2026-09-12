using VSlices.Space.Quantities;
using VSlices.Space.Quantities.Abstract;
using static VSlices.Space.Conversions;

namespace VSlices.Space.Tests;

public class ProductTests
{
    [Fact]
    public void homogeneous_length_product_converges_right_coordinate_to_left_basis()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(3m);

        Product<M.Length, M.Length, Kilometers, decimal> product = width * depth;

        Assert.Equal(0.006m, product.Value);
        Assert.IsAssignableFrom<Q<M.Mul<M.Length, M.Length>, Kilometers, decimal>>(product);
        Assert.Equal(1_000m, Kilometers.ReferenceScale);
    }

    [Fact]
    public void recommended_lengths_form_a_structural_product_without_becoming_area()
    {
        var width = new vLength(2d);
        var depth = new vLength(3d);

        var product = width * depth;

        Assert.IsType<Product<M.Length, M.Length, Meters, double>>(product);
        Assert.Equal(6d, product.Value);
    }

    [Fact]
    public void heterogeneous_product_preserves_independent_coordinate_bases_without_claiming_one_Q_coordinate()
    {
        var product = new Product<M.Mass, Kilograms, M.Length, Meters, decimal>(6m);

        Assert.Equal(6m, product.Value);
        Assert.Equal(1_000m, Kilograms.ReferenceScale);
        Assert.Equal(1m, Meters.ReferenceScale);
        Assert.IsNotAssignableFrom<Q<M.Mul<M.Mass, M.Length>, Kilograms, decimal>>(product);
    }

    [Fact]
    public void area_is_explicitly_established_from_the_length_product()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(3m);

        var structural = width * depth;
        Area<Kilometers, decimal> semantic = area(structural);
        Product<M.Length, M.Length, Kilometers, decimal> widened = WidenArea(semantic);

        Assert.Equal(0.006m, semantic.Value);
        Assert.Equal(structural, widened);
        Assert.IsAssignableFrom<Q<M.Mul<M.Length, M.Length>, Kilometers, decimal>>(semantic);
    }

    [Fact]
    public void area_symbol_is_declared_without_creating_a_squared_coordinate_type()
    {
        var attribute = Assert.Single(
            typeof(Area<Meters, double>)
                .GetCustomAttributes(typeof(AlgebraicSymbolAttribute), inherit: false)
                .Cast<AlgebraicSymbolAttribute>());

        Assert.Equal("area", attribute.Symbol);

        var width = new vLength(2d);
        var depth = new vLength(3d);

        Assert.IsType<Area<Meters, double>>(area(width * depth));
    }

    [Fact]
    public void area_times_length_aligns_the_primitive_basis_and_uses_compact_product()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(300m);
        var height = new ProbeLength<Meters, decimal>(500m);
        var surface = area(width * depth);

        Product<M.Mul<M.Length, M.Length>, M.Length, Kilometers, decimal> structural =
            surface * height;

        Assert.Equal(0.3m, structural.Value);
        Assert.IsAssignableFrom<
            Q<M.Mul<M.Mul<M.Length, M.Length>, M.Length>, Kilometers, decimal>>(structural);
    }

    [Fact]
    public void volume_is_explicitly_established_from_area_times_length()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(300m);
        var height = new ProbeLength<Meters, decimal>(500m);
        var surface = area(width * depth);
        var structural = surface * height;

        Volume<Kilometers, decimal> semantic = volume(structural);
        Product<M.Mul<M.Length, M.Length>, M.Length, Kilometers, decimal> widened =
            WidenVolume(semantic);

        Assert.Equal(0.3m, semantic.Value);
        Assert.Equal(structural, widened);
        Assert.IsAssignableFrom<
            Q<M.Mul<M.Mul<M.Length, M.Length>, M.Length>, Kilometers, decimal>>(semantic);
    }

    [Fact]
    public void volume_symbol_is_declared_without_creating_a_cubic_coordinate_type()
    {
        var attribute = Assert.Single(
            typeof(Volume<Meters, double>)
                .GetCustomAttributes(typeof(AlgebraicSymbolAttribute), inherit: false)
                .Cast<AlgebraicSymbolAttribute>());

        Assert.Equal("volume", attribute.Symbol);

        var width = new vLength(2d);
        var depth = new vLength(3d);
        var height = new vLength(4d);

        Assert.IsType<Volume<Meters, double>>(volume(area(width * depth) * height));
    }

    private static Product<M.Length, M.Length, C, T> WidenArea<C, T>(Area<C, T> area)
        where C : Coordinate<M.Length>
        where T : System.Numerics.INumber<T> =>
        area.ToBase();

    private static Product<M.Mul<M.Length, M.Length>, M.Length, C, T> WidenVolume<C, T>(Volume<C, T> volume)
        where C : Coordinate<M.Length>
        where T : System.Numerics.INumber<T> =>
        volume.ToBase();

    private sealed class ProbeLength<C, T> : Length<ProbeLength<C, T>, C, T>
        where C : Coordinate<M.Length>
        where T : System.Numerics.INumber<T>
    {
        public ProbeLength(T value) : base(value) { }
    }
}
