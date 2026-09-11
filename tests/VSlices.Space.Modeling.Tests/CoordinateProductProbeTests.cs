using VSlices.Space.Modeling.CoordinateProductProbe;

namespace VSlices.Space.Modeling.Tests;

public class CoordinateProductProbeTests
{
    [Fact]
    public void product_infers_both_families_coordinates_and_one_shared_carrier()
    {
        var mass = new ProbeMass<double>(2d);
        var length = new ProbeLength<double>(3d);

        ProductQuantity<
            Dimension.Mass,
            Kilograms,
            Dimension.Length,
            Meters,
            double> result = mass * length;

        Assert.Equal(6d, result.Value);
        Assert.IsAssignableFrom<Q<
            Dimension.Product<Dimension.Mass, Dimension.Length>,
            ProductCoordinate<Dimension.Mass, Kilograms, Dimension.Length, Meters>,
            double>>(result);
    }
}
