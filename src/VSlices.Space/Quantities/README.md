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

Every quantity belongs to a dimensional family, is expressed using a coordinate, and carries a numeric value:

```csharp
Q<F, C, T>
```

where:

```text
F = dimensional family
C = effective coordinate for that family
T = numeric carrier
```

For example:

```csharp
Q<Dimension.Length, Kilometers, double>
```

means a quantity of Length expressed in Kilometers with a `double` numeric carrier.

`C` replaces the earlier separation between unit and prefix. `Kilometers`, for example, is already the effective coordinate rather than `Meters + Kilo` being carried independently through every generic type.

For composed quantities, the public semantic type may expose only the primitive basis while its `Q` membership uses a lifted coordinate such as `ProductCoordinate<...>`.

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

This is why the model does not require types such as `SquaredKilometers` or `CubicMeters`.

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
    -> convert RIGHT coordinate to LEFT coordinate when a truthful conversion exists
    -> perform the operation
    -> express the result using LEFT's basis
```

The current multiplication examples follow the same principle where applicable.

## 5. Structural Product

`Product` represents structural dimensional multiplication. It does **not** by itself introduce domain semantics such as Area, Volume, Energy, or Torque.

Two Product shapes are currently useful because two materially different structural situations exist.

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

and `ProductCoordinate<Length,Length,Kilometers>` lifts the Kilometer basis into that composed dimension.

### 5.2 Full Product

The full Product form preserves a coordinate for each operand dimensional shape:

```csharp
Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>
```

It is necessary whenever the operand dimensions cannot be represented truthfully by the compact homogeneous shape.

A simple example is:

```text
Mass<Kilograms,T> * Length<Meters,T>
```

where `Meters` cannot be converted into `Kilograms`:

```csharp
Product<
    Dimension.Mass,
    Kilograms,
    Dimension.Length,
    Meters,
    T>
```

The Volume pressure revealed an important refinement: **using the full Product form does not imply that the primitive coordinate bases are unrelated.**

For example, `Area<Kilometers,T> * Length<Meters,T>` can convert the Length from Meters to Kilometers before multiplication, because both ultimately use Length as their primitive basis. But the operand dimensional shapes remain different:

```text
Area coordinate shape   = km²
Length coordinate shape = km
```

so the structural result still uses the full Product form:

```csharp
Product<
    Dimension.Product<Dimension.Length, Dimension.Length>,
    ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>,
    Dimension.Length,
    Kilometers,
    T>
```

In short:

```text
compact Product
    -> same dimensional family, one converged basis parameter is sufficient

full Product
    -> operand dimensional shapes remain independently represented
       even when their primitive bases can first be aligned
```

## 6. Representability does not grant operator authority

The fact that `Product<A,B,...>` can represent a structural result does not mean every multiplication is automatically available.

The current production surface explicitly exposes:

```text
Length * Length
Area   * Length
```

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

produces a structural Product. Semantic interpretation is established explicitly:

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

The same rule now applies to Volume:

```text
representable as (Length × Length) × Length
!=
semantically established as Volume
```

## 8. Area

`Area<C,T>` is a semantic quantity established from a homogeneous Length Product.

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
Product<Dimension.Length, Dimension.Length, C, T>
```

So:

```csharp
Area<Kilometers, decimal>
```

is naturally read in `km²` without introducing a separate `SquaredKilometers` type.

## 9. Volume

`Volume<C,T>` is established explicitly from `Area<C,T> * Length<...,T>`.

Suppose:

```text
surface = 0.6 km²
height  = 500 m
```

The authorized multiplication aligns the primitive Length basis first:

```text
500 m -> 0.5 km
0.6 km² * 0.5 km
= 0.3 km³
```

The structural result is still a full Product because Area and Length have different dimensional shapes:

```csharp
Product<
    Dimension.Product<Dimension.Length, Dimension.Length>,
    ProductCoordinate<Dimension.Length, Dimension.Length, Kilometers>,
    Dimension.Length,
    Kilometers,
    decimal>
```

Then:

```csharp
var semantic = volume(structural);
```

establishes:

```csharp
Volume<Kilometers, decimal>
```

whose `Q` membership has dimensional shape:

```text
(Length × Length) × Length
```

and an effective scale of:

```text
Kilometers² × Kilometers
= Kilometers³
```

Again, no `CubicKilometers` CLR type is required.

A complete pipeline now looks like:

```csharp
using static VSlices.Space.Quantities.Conversions;

var surface = area(width * depth);
var capacity = volume(surface * height);
```

The structural operators and semantic establishment functions remain separate steps.

## 10. `AlgebraicSymbol`

`[AlgebraicSymbol("area")]` and `[AlgebraicSymbol("volume")]` name explicit algebraic establishment operations.

They do not redefine the dimensional equations and they do not authorize implicit conversions.

The intended consumer syntax is:

```csharp
using static VSlices.Space.Quantities.Conversions;

var surface = area(width * depth);
var capacity = volume(surface * height);
```

The structural relationships are already represented by the type system. The symbols supply concise explicit operations for establishing semantic spaces.

Current working rule:

> **Source generation may complete a declared semantic relation; it must not invent the relation.**

The present `Conversions.area(...)` and `Conversions.volume(...)` implementations are manual and intentionally shaped like code a future source generator may emit.

## 11. What the model intentionally does not do

The current model intentionally avoids several convenient-looking shortcuts:

- it does not generate `SquaredMeters`, `SquaredKilometers`, `CubedFeet`, and similar coordinate types for every algebraic composition;
- it does not turn every structural Product into a semantic quantity;
- it does not automatically expose every mathematically representable multiplication operator;
- it does not implicitly promote different numeric carriers during Product formation;
- it does not use implicit `Product -> Area` or `Product -> Volume` conversions;
- it does not treat dimensional equivalence as semantic identity.

These limits are features of the model, not missing convenience APIs.

## 12. Current boundaries and open questions

This surface is still being discovered through executable examples. Current evidence says:

- homogeneous quantities can converge the right coordinate into the left coordinate before multiplication;
- composed semantic quantities can still expose a primitive basis such as `Kilometers` while their `Q` coordinate is structurally lifted;
- full Product is about preserving different operand dimensional shapes, not necessarily about primitive bases being impossible to align;
- operator authority remains explicit and separate from structural representability;
- Area and Volume currently forget their semantic name when widened to their structural Product base; whether later algebra requires preserving semantic operands such as `Area` inside a Product remains open;
- Power, Quotient, dimensional normalization, equivalence, and source-generated algebraic symbols remain later work.

The implementation should continue to be treated as evidence about the model rather than as a reason to force the model around current C# mechanics.
