# Quantities

This folder contains the current quantity model used by `VSlices.Space`.

The model is intentionally small and semantic-first. It distinguishes:

- **what is being measured**;
- **how that quantity is expressed**;
- **how the numeric value is carried**;
- **which algebraic compositions are structurally representable**;
- **which semantic interpretations are explicitly established**.

The guiding rule is:

> **Dimension tells us what is composed. Coordinate tells us how the basis is measured.**

That distinction is the key to reading the API.

## 1. The core shape: `Q<F, C, T>`

Every quantity belongs to a dimensional family, is expressed using a coordinate basis, and carries a numeric value:

```csharp
Q<F, C, T>
```

where:

```text
F = dimensional family
C = coordinate basis
T = numeric carrier
```

For example:

```csharp
Q<Dimension.Length, Kilometers, double>
```

means:

> a quantity of Length, expressed using Kilometers as its coordinate basis, with a `double` numeric carrier.

`C` replaces the earlier separation between unit and prefix. `Kilometers`, for example, is already the effective coordinate rather than `Meters + Kilo` being carried independently through every generic type.

## 2. Base quantities

The current built-in dimensional families are:

```csharp
Dimension.Mass
Dimension.Length
Dimension.Duration
```

The corresponding quantity families are `Mass`, `Length`, and `Duration`.

For example:

```csharp
Length<Kilometers, decimal, MyLength>
Mass<Kilograms, double, MyMass>
Duration<Seconds, double, MyDuration>
```

The extra `SELF` parameter used by these nominal families is not part of `Q<F,C,T>`. It exists so arithmetic can preserve the exact nominal type after re-materialization.

Conceptually:

```text
Q<F,C,T>
    describes quantity-family membership

SELF
    describes nominal closure / re-materialization
```

## 3. Coordinates

A coordinate belongs to one dimensional family and provides a scale relative to that family's canonical coordinate:

```csharp
public interface Coordinate<F>
    where F : Dimension
{
    static abstract decimal Scale { get; }
}
```

Examples include:

```text
Mass:      Grams, Kilograms, Micrograms, Pounds
Length:    Meters, Kilometers, Micrometers, Feet
Duration:  Seconds, Minutes, Microseconds
```

Coordinates describe a **basis of expression**. They do not have to encode the whole algebraic shape of a composed dimension.

This becomes important for Area.

## 4. Same-family arithmetic converges to the left coordinate

Addition and subtraction between quantities of the same dimensional family are left-biased.

For example:

```text
2 km + 300 m
```

is interpreted as:

```text
300 m -> 0.3 km
2 km + 0.3 km
= 2.3 km
```

The result therefore keeps the left coordinate.

This gives a useful general rule:

```text
LEFT operation RIGHT
    -> convert RIGHT coordinate to LEFT coordinate when the family allows it
    -> perform the operation
    -> express the result using LEFT coordinate
```

The current homogeneous multiplication pressure follows the same principle.

## 5. Structural Product

`Product` represents structural dimensional multiplication. It does **not** by itself introduce domain semantics such as Area, Volume, Energy, or Torque.

Two Product shapes are currently useful because two materially different coordinate situations exist.

### 5.1 Homogeneous / coordinate-converged Product

When both operands belong to the same convertible dimensional family, the right coordinate can be converted to the left coordinate before multiplication.

```text
Q<F,C1,T> * Q<F,C2,T>
    -> convert C2 to C1
    -> Product<F,F,C1,T>
```

For example:

```text
2 km * 3 m
```

becomes:

```text
3 m -> 0.003 km
2 km * 0.003 km
= 0.006 km²
```

The structural result is represented as:

```csharp
Product<
    Dimension.Length,
    Dimension.Length,
    Kilometers,
    decimal>
```

Notice that the coordinate parameter is still `Kilometers`, not `SquaredKilometers`.

The squared shape is already described by:

```csharp
Dimension.Product<
    Dimension.Length,
    Dimension.Length>
```

The coordinate only tells us the basis used to express that shape.

So:

```text
Product<Length,Length,Kilometers,T>
```

is read as:

```text
Length × Length expressed on the Kilometer basis
= km²
```

This avoids needing an ever-growing family of backing types such as:

```text
SquaredMeters
SquaredKilometers
CubedFeet
...
```

### 5.2 Heterogeneous Product

When the dimensions differ, there is no general conversion from the right coordinate to the left coordinate.

For example:

```text
Mass<Kilograms,T> * Length<Meters,T>
```

cannot convert `Meters` into `Kilograms`.

The structural result therefore preserves both coordinate bases:

```csharp
Product<
    Dimension.Mass,
    Kilograms,
    Dimension.Length,
    Meters,
    T>
```

This is structurally different from the homogeneous Product because the two coordinate bases cannot be collapsed truthfully.

In short:

```text
homogeneous product
    -> one converged coordinate basis

heterogeneous product
    -> two independently preserved coordinate bases
```

## 6. Representability does not grant operator authority

The fact that `Product<A,B,...>` can represent a structural result does not mean every multiplication is automatically available.

The current production surface explicitly exposes `Length * Length`.

This is deliberate:

```text
Product can represent A × B
!=
A × B is authorized
```

Algebraic syntax remains deny-by-default. A relationship must be intentionally exposed rather than inferred merely because the result type is structurally representable.

## 7. Structural quantity is not semantic quantity

A structural `Length × Length` result is not automatically an `Area`.

```csharp
var structural = width * depth;
```

produces a structural Product:

```csharp
Product<
    Dimension.Length,
    Dimension.Length,
    Kilometers,
    decimal>
```

It does not directly produce:

```csharp
Area<Kilometers, decimal>
```

Semantic interpretation is established explicitly:

```csharp
using static VSlices.Space.Quantities.Conversions;

Area<Kilometers, decimal> surface = area(structural);
```

The distinction is intentional:

```text
representable as Length × Length
!=
semantically established as Area
```

## 8. Area

`Area<C,T>` is the first semantic quantity established from a structural Product.

Its current shape is equivalent to:

```csharp
[AlgebraicSymbol("area")]
Area<C,T>
    : Q<
        Dimension.Product<Dimension.Length,Dimension.Length>,
        ProductCoordinate<Dimension.Length,Dimension.Length,C>,
        T>
```

and it is a `DerivedSpace` of:

```csharp
Product<
    Dimension.Length,
    Dimension.Length,
    C,
    T>
```

So:

```csharp
Area<Kilometers, decimal>
```

means an Area whose structural dimensional shape is `Length × Length`, expressed on the Kilometer basis, using `decimal` as its numeric carrier.

It is naturally read as a value in `km²` without introducing a separate `SquaredKilometers` type.

### Worked example

The library does not currently expose convenience types named `KilometerLength` or `MeterLength`; the following local types only make the example readable:

```csharp
sealed class KilometerLength(decimal value)
    : Length<Kilometers, decimal, KilometerLength>(value);

sealed class MeterLength(decimal value)
    : Length<Meters, decimal, MeterLength>(value);
```

Now:

```csharp
var width = new KilometerLength(2m);
var depth = new MeterLength(300m);
```

Their structural multiplication follows the left-coordinate rule:

```text
300 m -> 0.3 km
2 km * 0.3 km
= 0.6 km²
```

Then:

```csharp
var structural = width * depth;
```

has the structural shape:

```text
Product<Length,Length,Kilometers,decimal>
Value = 0.6
```

and:

```csharp
var surface = area(structural);
```

establishes:

```text
Area<Kilometers,decimal>
Value = 0.6
```

The numeric value and coordinate basis are preserved; only the semantic interpretation becomes more specific.

## 9. `AlgebraicSymbol`

`[AlgebraicSymbol("area")]` names an explicit algebraic establishment operation.

It does not redefine the dimensional equation and it does not authorize an implicit conversion.

The intended consumer syntax is:

```csharp
using static VSlices.Space.Quantities.Conversions;

var surface = area(width * depth);
```

The structural relationship is already represented by the type system. The symbol supplies a concise explicit operation for establishing the semantic space.

Current working rule:

> **Source generation may complete a declared semantic relation; it must not invent the relation.**

The present `Conversions.area(...)` implementation is manual and intentionally shaped like the code a future source generator may emit.

## 10. What the model intentionally does not do

The current model intentionally avoids several convenient-looking shortcuts:

- it does not generate `SquaredMeters`, `SquaredKilometers`, `CubedFeet`, and similar coordinate types for every algebraic composition;
- it does not turn every structural Product into a semantic quantity;
- it does not automatically expose every mathematically representable multiplication operator;
- it does not implicitly promote different numeric carriers during Product formation;
- it does not use an implicit `Product -> Area` conversion;
- it does not treat dimensional equivalence as semantic identity.

These limits are features of the model, not missing convenience APIs.

## 11. Current boundaries and open questions

This surface is still being discovered through executable examples. In particular:

- the homogeneous Product currently assumes a shared coordinate basis can be truthfully reused after convergence;
- the heterogeneous Product preserves both coordinate bases because no common basis exists by default;
- operator authority remains explicit and separate from structural representability;
- `Area * Length -> Volume` is the next pressure case and may reveal whether a semantic quantity such as Area should remain visible as an operand or must expose an explicit structural expansion;
- Power, Quotient, dimensional normalization, equivalence, and source-generated algebraic symbols remain later work.

The implementation should continue to be treated as evidence about the model rather than as a reason to force the model around current C# mechanics.
