using static VSlices.Space.Modeling.AlgebraicSymbolAreaVolumeProbe.Conversions;
using VSlices.Space.Modeling.AlgebraicSymbolAreaVolumeProbe;

namespace VSlices.Space.Modeling.Tests;

public class AlgebraicSymbolAreaVolumeProbeTests
{
    [Fact]
    public void explicit_area_symbol_wraps_the_structural_length_product()
    {
        var width = new Length<double>(2d);
        var height = new Length<double>(3d);

        Product<Length<double>, Length<double>, double> structural = width * height;
        Area<double> semantic = area(structural);

        Assert.Equal(6d, structural.Value);
        Assert.Equal(6d, semantic.Value);
        Assert.Equal(structural, semantic.ToBase());
    }

    [Fact]
    public void semantic_area_can_participate_in_the_next_declared_product()
    {
        var width = new Length<double>(2d);
        var depth = new Length<double>(3d);
        var height = new Length<double>(4d);

        Area<double> baseArea = area(width * depth);
        Product<Area<double>, Length<double>, double> structural = baseArea * height;
        Volume<double> semantic = volume(structural);

        Assert.Equal(6d, baseArea.Value);
        Assert.Equal(24d, structural.Value);
        Assert.Equal(24d, semantic.Value);
        Assert.Equal(structural, semantic.ToBase());
    }

    [Fact]
    public void product_keeps_the_declared_semantic_operand_instead_of_forcing_expansion()
    {
        var width = new Length<decimal>(2m);
        var depth = new Length<decimal>(3m);
        var height = new Length<decimal>(4m);

        Area<decimal> baseArea = area(width * depth);
        Product<Area<decimal>, Length<decimal>, decimal> structural = baseArea * height;

        Assert.Equal(24m, structural.Value);
        Assert.IsType<Product<Area<decimal>, Length<decimal>, decimal>>(structural);
    }
}
