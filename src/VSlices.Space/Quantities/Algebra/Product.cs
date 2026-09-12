using System.Numerics;
using VSlices.Space.Quantities.Abstract;

namespace VSlices.Space.Quantities;

/// <summary>
/// Structural multiplication whose operands can be expressed on one shared
/// primitive coordinate basis C. The algebraic shape lives in M.Mul; C only names
/// the basis used to express the resulting magnitude.
/// </summary>
public sealed record Product<LEFT_F, RIGHT_F, C, T>(T Value) :
    Q<M.Mul<LEFT_F, RIGHT_F>, C, T>,
    DiscreteSpace<Product<LEFT_F, RIGHT_F, C, T>>
    where LEFT_F : M
    where RIGHT_F : M
    where C : Coordinate
    where T : INumber<T>;

/// <summary>
/// Structural multiplication whose operand coordinate bases remain independently
/// represented. Because there is no single truthful C, this form intentionally does
/// not implement Q&lt;F,C,T&gt;.
/// </summary>
public sealed record Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>(T Value) :
    DiscreteSpace<Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>>
    where LEFT_F : M
    where LEFT_C : Coordinate
    where RIGHT_F : M
    where RIGHT_C : Coordinate
    where T : INumber<T>;
