namespace VSlices.Space.Quantities.Abstract;

/// <summary>
/// Algebraic magnitude language used by quantity values.
/// Magnitude shape is independent from the coordinate basis used to express a value.
/// </summary>
public abstract class M
{
    private M()
    {
    }

    public sealed class Mass : M
    {
        private Mass() { }
    }

    public sealed class Length : M
    {
        private Length() { }
    }

    public sealed class Duration : M
    {
        private Duration() { }
    }

    /// <summary>
    /// Structural multiplication of two magnitudes.
    /// </summary>
    public sealed class Mul<LEFT, RIGHT> : M
        where LEFT : M
        where RIGHT : M
    {
        private Mul() { }
    }

    /// <summary>
    /// Structural division of two magnitudes.
    /// </summary>
    public sealed class Div<LEFT, RIGHT> : M
        where LEFT : M
        where RIGHT : M
    {
        private Div() { }
    }

    /// <summary>
    /// Structural exponentiation of a magnitude.
    /// </summary>
    public sealed class Pow<BASE, EXPONENT> : M
        where BASE : M
        where EXPONENT : Exponent
    {
        private Pow() { }
    }
}
