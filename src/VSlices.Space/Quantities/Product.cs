using System.Numerics;

namespace VSlices.Space.Quantities;

/// <summary>
/// Canonical structural result of an explicitly available multiplication.
/// Product preserves the dimensions and effective coordinates of both operands
/// and requires one converged numeric carrier T.
///
/// This type does not introduce domain semantics such as Area, Volume, Energy,
/// or Torque. Those belong to separately established semantic spaces.
/// </summary>
public sealed record Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>(T Value) :
    Q<
        Dimension.Product<LEFT_F, RIGHT_F>,
        ProductCoordinate<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C>,
        T>
    where LEFT_F : Dimension
    where LEFT_C : Coordinate<LEFT_F>
    where RIGHT_F : Dimension
    where RIGHT_C : Coordinate<RIGHT_F>
    where T : INumber<T>;
