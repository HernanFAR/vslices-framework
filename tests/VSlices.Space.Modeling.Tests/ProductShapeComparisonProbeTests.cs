using static VSlices.Space.Modeling.ProductShapeComparisonProbe.ProductComparison;
using VSlices.Space.Modeling.ProductShapeComparisonProbe;

namespace VSlices.Space.Modeling.Tests;

public class ProductShapeComparisonProbeTests
{
    [Fact]
    public void A_keeps_operand_decomposition_in_the_quantity_type()
    {
        var mass = new ProbeMass<double>(2d);
        var length = new ProbeLength<double>(3d);

        ProductA<Dimension.Mass, Kilograms, Dimension.Length, Meters, double> result =
            MultiplyA(mass, length);

        Assert.Equal(6d, result.Value);
    }

    [Fact]
    public void B_moves_operand_decomposition_into_composed_family_and_coordinate()
    {
        var mass = new ProbeMass<double>(2d);
        var length = new ProbeLength<double>(3d);

        ProductB<
            Dimension.Product<Dimension.Mass, Dimension.Length>,
            ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
            double> result = MultiplyB(mass, length);

        Assert.Equal(6d, result.Value);
        Assert.IsAssignableFrom<Q<
            Dimension.Product<Dimension.Mass, Dimension.Length>,
            ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
            double>>(result);
    }

    [Fact]
    public void C_erases_quantity_level_product_but_preserves_the_same_semantic_coordinate()
    {
        var mass = new ProbeMass<double>(2d);
        var length = new ProbeLength<double>(3d);

        StructuralQuantity<
            Dimension.Product<Dimension.Mass, Dimension.Length>,
            ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
            double> result = MultiplyC(mass, length);

        Assert.Equal(6d, result.Value);
        Assert.IsAssignableFrom<Q<
            Dimension.Product<Dimension.Mass, Dimension.Length>,
            ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
            double>>(result);
    }

    [Fact]
    public void B_and_C_expose_the_same_Q_membership_even_though_their_nominal_result_shapes_differ()
    {
        var mass = new ProbeMass<decimal>(2m);
        var length = new ProbeLength<decimal>(3m);

        Q<
            Dimension.Product<Dimension.Mass, Dimension.Length>,
            ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
            decimal> b = MultiplyB(mass, length);

        Q<
            Dimension.Product<Dimension.Mass, Dimension.Length>,
            ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
            decimal> c = MultiplyC(mass, length);

        Assert.Equal(b.Value, c.Value);
        Assert.Equal(6m, b.Value);
    }
}
