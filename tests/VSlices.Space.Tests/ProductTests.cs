using VSlices.Space.Quantities;
using static VSlices.Space.Quantities.Conversions;

namespace VSlices.Space.Tests;

public class ProductTests
{
    [Fact]
    public void length_multiplication_preserves_structural_dimension_coordinate_and_carrier()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(3m);

        Product<
            Dimension.Length,
            Kilometers,
            Dimension.Length,
            Meters,
            decimal> product = width * depth;

        Assert.Equal(6m, product.Value);
        Assert.IsAssignableFrom<
            Q<
                Dimension.Product<Dimension.Length, Dimension.Length>,
                ProductCoordinate<Dimension.Length, Kilometers, Dimension.Length, Meters>,
                decimal>>(product);
        Assert.Equal(1_000m,
            ProductCoordinate<Dimension.Length, Kilometers, Dimension.Length, Meters>.Scale);
    }

    [Fact]
    public void recommended_lengths_can_form_a_structural_product_without_becoming_area()
    {
        var width = new vLength(2d);
        var depth = new vLength(3d);

        var product = width * depth;

        Assert.IsType<Product<
            Dimension.Length,
            Meters,
            Dimension.Length,
            Meters,
            double>>(product);
        Assert.Equal(6d, product.Value);
    }

    [Fact]
    public void area_is_explicitly_established_from_the_length_product()
    {
        var width = new ProbeLength<Kilometers, decimal>(2m);
        var depth = new ProbeLength<Meters, decimal>(3m);

        var structural = width * depth;
        Area<Kilometers, Meters, decimal> semantic = area(structural);

        Assert.Equal(6m, semantic.Value);
        Assert.Equal(structural, semantic.ToBase());
        Assert.IsAssignableFrom<
            Q<
                Dimension.Product<Dimension.Length, Dimension.Length>,
                ProductCoordinate<Dimension.Length, Kilometers, Dimension.Length, Meters>,
                decimal>>(semantic);
        Assert.IsAssignableFrom<
            DerivedSpace<
                Area<Kilometers, Meters, decimal>,
                Product<Dimension.Length, Kilometers, Dimension.Length, Meters, decimal>>>(semantic);
    }

    [Fact]
    public void area_symbol_is_declared_without_changing_the_structural_product()
    {
        var attribute = Assert.Single(
            typeof(Area<Meters, Meters, double>)
                .GetCustomAttributes(typeof(AlgebraicSymbolAttribute), inherit: false)
                .Cast<AlgebraicSymbolAttribute>());

        Assert.Equal("area", attribute.Symbol);

        var width = new vLength(2d);
        var depth = new vLength(3d);
        var structural = width * depth;

        Assert.IsType<Product<
            Dimension.Length,
            Meters,
            Dimension.Length,
            Meters,
            double>>(structural);
        Assert.IsType<Area<Meters, Meters, double>>(area(structural));
    }

    private sealed class ProbeLength<C, T> : Length<C, T, ProbeLength<C, T>>
        where C : Coordinate<Dimension.Length>
        where T : System.Numerics.INumber<T>
    {
        public ProbeLength(T value) : base(value)
        {
        }
    }
}
