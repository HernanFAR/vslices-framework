using VSlices.Space.Quantities;
using VSlices.Space.Quantities.Abstract;
using VSlices.Space.Quantities.Algebra;
using VSlices.Space.Quantities.Derived;
using static VSlices.Space.Quantities.Conversions;

namespace VSlices.Space.Tests;

public class ProductTests
{
    [Fact]
    public void homogeneous_length_product_converges_right_coordinate_to_left_basis()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(3m);

        Product<
            Dimension.Length,
            Dimension.Length,
            Kilometers,
            decimal> product = width * depth;

        Assert.Equal(0.006m, product.Value);
        Assert.IsAssignableFrom<
            Q<
                Dimension.Product<Dimension.Length, Dimension.Length>,
                ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>,
                decimal>>(product);
        Assert.Equal(1_000_000m,
            ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>.Scale);
    }

    [Fact]
    public void recommended_lengths_form_a_homogeneous_structural_product_without_becoming_area()
    {
        var width = new vLength(2d);
        var depth = new vLength(3d);

        var product = width * depth;

        Assert.IsType<Product<
            Dimension.Length,
            Dimension.Length,
            Meters,
            double>>(product);
        Assert.Equal(6d, product.Value);
    }

    [Fact]
    public void heterogeneous_product_preserves_independent_coordinate_bases()
    {
        var product = new Product<
            Dimension.Mass,
            Kilograms,
            Dimension.Length,
            Meters,
            decimal>(6m);

        Assert.IsAssignableFrom<
            Q<
                Dimension.Product<Dimension.Mass, Dimension.Length>,
                ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
                decimal>>(product);
        Assert.Equal(1_000m,
            ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>.Scale);
    }

    [Fact]
    public void area_is_explicitly_established_from_the_homogeneous_length_product()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(3m);

        var structural = width * depth;
        Area<Kilometers, decimal> semantic = area(structural);
        Product<Dimension.Length, Dimension.Length, Kilometers, decimal> widened =
            WidenArea(semantic);

        Assert.Equal(0.006m, semantic.Value);
        Assert.Equal(structural, widened);
        Assert.IsAssignableFrom<
            Q<
                Dimension.Product<Dimension.Length, Dimension.Length>,
                ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>,
                decimal>>(semantic);
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
        var structural = width * depth;

        Assert.IsType<Product<
            Dimension.Length,
            Dimension.Length,
            Meters,
            double>>(structural);
        Assert.IsType<Area<Meters, double>>(area(structural));
    }

    [Fact]
    public void area_times_length_aligns_the_primitive_length_basis_but_keeps_full_product_shape()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(300m);
        var height = new ProbeLength<Meters, decimal>(500m);
        var surface = area(width * depth);

        Product<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>,
            Dimension.Length,
            Kilometers,
            decimal> structural = surface * height;

        Assert.Equal(0.3m, structural.Value);
        Assert.Equal(1_000_000_000m,
            ProductCoordinate<
                Dimension.Product<Dimension.Length, Dimension.Length>,
                ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>,
                Dimension.Length,
                Kilometers>.Scale);
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
        Product<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>,
            Dimension.Length,
            Kilometers,
            decimal> widened = WidenVolume(semantic);

        Assert.Equal(0.3m, semantic.Value);
        Assert.Equal(structural, widened);
        Assert.IsAssignableFrom<
            Q<
                Dimension.Product<
                    Dimension.Product<Dimension.Length, Dimension.Length>,
                    Dimension.Length>,
                ProductCoordinate<
                    Dimension.Product<Dimension.Length, Dimension.Length>,
                    ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>,
                    Dimension.Length,
                    Kilometers>,
                decimal>>(semantic);
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

        Assert.IsType<Volume<Meters, double>>(
            volume(area(width * depth) * height));
    }

    private static Product<Dimension.Length, Dimension.Length, C, T> WidenArea<C, T>(Area<C, T> area)
        where C : Coordinate<Dimension.Length>
        where T : System.Numerics.INumber<T> =>
        area.ToBase();

    private static Product<
        Dimension.Product<Dimension.Length, Dimension.Length>,
        ProductCoordinate<Dimension.Length, Dimension.Length, C>,
        Dimension.Length,
        C,
        T> WidenVolume<C, T>(Volume<C, T> volume)
        where C : Coordinate<Dimension.Length>
        where T : System.Numerics.INumber<T> =>
        volume.ToBase();

    private sealed class ProbeLength<C, T> : Length<C, T, ProbeLength<C, T>>
        where C : Coordinate<Dimension.Length>
        where T : System.Numerics.INumber<T>
    {
        public ProbeLength(T value) : base(value)
        {
        }
    }
}
