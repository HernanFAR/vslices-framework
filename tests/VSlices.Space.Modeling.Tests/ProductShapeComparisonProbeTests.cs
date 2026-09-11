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

    [Fact]
    public void length_times_length_preserves_effective_meter_coordinates_in_all_candidates()
    {
        var left = new ProbeLength<double>(2d);
        var right = new ProbeLength<double>(3d);

        ProductA<Dimension.Length, Meters, Dimension.Length, Meters, double> a =
            SquareA(left, right);

        ProductB<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
            double> b = SquareB(left, right);

        StructuralQuantity<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
            double> c = SquareC(left, right);

        Assert.Equal(6d, a.Value);
        Assert.Equal(a.Value, b.Value);
        Assert.Equal(a.Value, c.Value);
    }

    [Fact]
    public void nested_product_makes_structure_growth_visible_without_changing_the_shared_carrier()
    {
        var width = new ProbeLength<double>(2d);
        var height = new ProbeLength<double>(3d);
        var mass = new ProbeMass<double>(4d);

        var areaA = SquareA(width, height);
        var areaB = SquareB(width, height);
        var areaC = SquareC(width, height);

        ProductA<
            Dimension.Product<Dimension.Length, Dimension.Length>,
            ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
            Dimension.Mass,
            Kilograms,
            double> a = NestedA(areaA, mass);

        ProductB<
            Dimension.Product<Dimension.Product<Dimension.Length, Dimension.Length>, Dimension.Mass>,
            ProductCoordinate<
                Dimension.Product<Dimension.Length, Dimension.Length>,
                ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
                Dimension.Mass,
                Kilograms>,
            double> b = NestedB(areaB, mass);

        StructuralQuantity<
            Dimension.Product<Dimension.Product<Dimension.Length, Dimension.Length>, Dimension.Mass>,
            ProductCoordinate<
                Dimension.Product<Dimension.Length, Dimension.Length>,
                ProductCoordinate<Dimension.Length, Meters, Dimension.Length, Meters>,
                Dimension.Mass,
                Kilograms>,
            double> c = NestedC(areaC, mass);

        Assert.Equal(24d, a.Value);
        Assert.Equal(a.Value, b.Value);
        Assert.Equal(a.Value, c.Value);
    }
}
