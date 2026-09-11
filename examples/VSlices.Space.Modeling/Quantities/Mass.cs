using VSlices.Arrows;
using VSlices.Space.Traits;

namespace VSlices.Space.Modeling.Quantities;

/// <summary>
/// Provisional canonical mass Value built on the generic quantity mechanism.
/// The vMass name is intentionally temporary while the quantity surface remains experimental.
/// Its canonical coordinate is currently kilo-scaled mass.
/// </summary>
public readonly record struct vMass :
    QuantitySpace<vMass, double>,
    Transformable<Quantity<MassDimension, Kilo>, vMass>
{
    private readonly Quantity<MassDimension, Kilo> quantity;

    private vMass(Quantity<MassDimension, Kilo> quantity) =>
        this.quantity = quantity;

    public double CanonValue =>
        quantity.CanonValue;

    public static Req<Quantity<MassDimension, Kilo>, vMass>.Full Transformation =>
        Req<Quantity<MassDimension, Kilo>, vMass>.Transform<Quantity<MassDimension, Kilo>, vMass>(
            quantity => new vMass(quantity));

    public static vMass AdditiveIdentity =>
        new(Quantity<MassDimension, Kilo>.AdditiveIdentity);

    public int CompareTo(vMass other) =>
        CanonValue.CompareTo(other.CanonValue);

    public Quantity<MassDimension, PREFIX> In<PREFIX>()
        where PREFIX : struct, Prefix<PREFIX> =>
        quantity.In<PREFIX>();

    public static vMass operator +(
        vMass left,
        vMass right) =>
        new(left.quantity + right.quantity);

    public static vMass operator -(
        vMass left,
        vMass right) =>
        new(left.quantity - right.quantity);

    public static vMass operator -(
        vMass value) =>
        new(-value.quantity);

    public static vMass operator *(
        vMass mass,
        double scalar) =>
        new(mass.quantity * scalar);

    public static vMass operator /(
        vMass mass,
        double scalar) =>
        new(mass.quantity / scalar);

    public static double operator /(
        vMass left,
        vMass right) =>
        left.CanonValue / right.CanonValue;

    public static bool operator <(
        vMass left,
        vMass right) =>
        left.CanonValue < right.CanonValue;

    public static bool operator >(
        vMass left,
        vMass right) =>
        left.CanonValue > right.CanonValue;

    public static bool operator <=(
        vMass left,
        vMass right) =>
        left.CanonValue <= right.CanonValue;

    public static bool operator >=(
        vMass left,
        vMass right) =>
        left.CanonValue >= right.CanonValue;
}
