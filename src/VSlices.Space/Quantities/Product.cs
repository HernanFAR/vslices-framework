using System.Numerics;

namespace VSlices.Space.Quantities;

/// <summary>
/// Canonical structural result for a homogeneous multiplication whose operand
/// coordinates were first converged to one shared base coordinate C.
/// The algebraic shape is carried by Dimension.Product; C remains the base
/// coordinate used to express that shape (for example Kilometers -> km²).
/// </summary>
public sealed record Product<LEFT_F, RIGHT_F, C, T>(T Value) :
    Q<
        Dimension.Product<LEFT_F, RIGHT_F>,
        ProductCoordinate<LEFT_F, RIGHT_F, C>,
        T>,
    DiscreteSpace<Product<LEFT_F, RIGHT_F, C, T>>
    where LEFT_F : Dimension
    where RIGHT_F : Dimension
    where C : Coordinate<LEFT_F>
    where T : INumber<T>;

/// <summary>
/// Canonical structural result for a heterogeneous multiplication where the
/// operands cannot share one coordinate basis. Both coordinates remain visible.
/// </summary>
public sealed record Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>(T Value) :
    Q<
        Dimension.Product<LEFT_F, RIGHT_F>,
        ProductCoordinate<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C>,
        T>,
    DiscreteSpace<Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>>
    where LEFT_F : Dimension
    where LEFT_C : Coordinate<LEFT_F>
    where RIGHT_F : Dimension
    where RIGHT_C : Coordinate<RIGHT_F>
    where T : INumber<T>;
