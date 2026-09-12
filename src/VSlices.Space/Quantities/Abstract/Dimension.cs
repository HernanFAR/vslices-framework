namespace VSlices.Space.Quantities.Abstract;

/// <summary>
/// Identifies a dimensional quantity family.
/// </summary>
public abstract class Dimension
{
    private Dimension()
    {
    }

    public sealed class Mass : Dimension
    {
        private Mass()
        {
        }
    }

    public sealed class Length : Dimension
    {
        private Length()
        {
        }
    }

    public sealed class Duration : Dimension
    {
        private Duration()
        {
        }
    }

    /// <summary>
    /// Structural dimensional multiplication. The operands remain visible in
    /// the type; no semantic interpretation such as Area or Energy is implied.
    /// </summary>
    public sealed class Product<LEFT, RIGHT> : Dimension
        where LEFT : Dimension
        where RIGHT : Dimension
    {
        private Product()
        {
        }
    }
}
