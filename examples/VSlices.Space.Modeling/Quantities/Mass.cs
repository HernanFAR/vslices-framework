using VSlices.Arrows;
using VSlices.Space.Traits;

namespace VSlices.Space.Modeling.Quantities;

/// <summary>
/// Canonical mass Value built on the generic quantity mechanism.
/// Its canonical coordinate is currently kilo-scaled mass.
/// </summary>
public readonly record struct Mass :
    QuantitySpace<Mass, double>,
    Transformable<Quantity<MassDimension, Kilo>, Mass>
{
    private readonly Quantity<MassDimension, Kilo> quantity;

    private Mass(Quantity<MassDimension, Kilo> quantity) =>
        this.quantity = quantity;

    public double CanonValue =>
        quantity.CanonValue;

    public static Req<Quantity<MassDimension, Kilo>, Mass>.Full Transformation =>
        Req<Quantity<MassDimension, Kilo>, Mass>.Transform<Quantity<MassDimension, Kilo>, Mass>(
            quantity => new Mass(quantity));

    public static Mass AdditiveIdentity =>
        new(Quantity<MassDimension, Kilo>.AdditiveIdentity);

    public int CompareTo(Mass other) =>
        CanonValue.CompareTo(other.CanonValue);

    public Quantity<MassDimension, PREFIX> In<PREFIX>()
        where PREFIX : struct, Prefix<PREFIX> =>
        quantity.In<PREFIX>();

    public static Mass operator +(
        Mass left,
        Mass right) =>
        new(left.quantity + right.quantity);

    public static Mass operator -(
        Mass left,
        Mass right) =>
        new(left.quantity - right.quantity);

    public static Mass operator -(
        Mass value) =>
        new(-value.quantity);

    public static Mass operator *(
        Mass mass,
        double scalar) =>
        new(mass.quantity * scalar);

    public static Mass operator /(
        Mass mass,
        double scalar) =>
        new(mass.quantity / scalar);

    public static double operator /(
        Mass left,
        Mass right) =>
        left.CanonValue / right.CanonValue;

    public static bool operator <(
        Mass left,
        Mass right) =>
        left.CanonValue < right.CanonValue;

    public static bool operator >(
        Mass left,
        Mass right) =>
        left.CanonValue > right.CanonValue;

    public static bool operator <=(
        Mass left,
        Mass right) =>
        left.CanonValue <= right.CanonValue;

    public static bool operator >=(
        Mass left,
        Mass right) =>
        left.CanonValue >= right.CanonValue;
}
