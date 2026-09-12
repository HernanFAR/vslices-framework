# Quantities

This folder contains the current quantity model used by `VSlices.Space`.

The model separates three concerns:

```text
M            algebraic magnitude shape
Coordinate   primitive measurement basis
T            numeric carrier
```

A quantity value is represented by:

```csharp
Q<F, C, T>
```

where `F : M`, `C : Coordinate`, and `T` is the numeric carrier.

The guiding rule is:

> **M tells us what is composed. Coordinate tells us how the primitive basis is measured.**

## Magnitudes

The algebraic magnitude vocabulary is intentionally compact:

```csharp
M.Mass
M.Length
M.Duration

M.Mul<A, B>
M.Div<A, B>
M.Pow<A, N>
```

`M.Mul`, `M.Div`, and `M.Pow` describe algebraic shape. They do not themselves authorize operators or establish domain semantics such as Area, Volume, Energy, or Velocity.

For example:

```csharp
M.Mul<M.Length, M.Length>
```

means `Length x Length`; it does not by itself mean Area.

## Coordinates

Coordinates represent primitive measurement bases:

```csharp
Coordinate
Coordinate<F> : Coordinate
```

Examples:

```csharp
Kilometers : Coordinate<M.Length>
Kilograms  : Coordinate<M.Mass>
Seconds    : Coordinate<M.Duration>
```

A coordinate exposes `ReferenceScale` with one fixed orientation:

```text
1 coordinate unit
=
ReferenceScale x canonical reference unit
```

VSlices currently fixes these canonical references:

```text
Mass      -> Gram
Length    -> Meter
Duration  -> Second
```

Therefore:

```text
Grams.ReferenceScale      = 1
Kilograms.ReferenceScale  = 1000
Pounds.ReferenceScale     = 453.59237

Meters.ReferenceScale     = 1
Kilometers.ReferenceScale = 1000

Seconds.ReferenceScale    = 1
Minutes.ReferenceScale    = 60
```

The reference coordinate is currently a VSlices convention, not a customizable setting.

The reference coordinate is also not the same thing as a preferred display/default coordinate. For example, recommended `Mass<T>` uses Kilograms while Mass reference scaling is defined relative to Grams.

## Coordinate is not algebraic shape

Coordinates do not encode squares, cubes, products, quotients, or powers.

This is deliberate.

```csharp
Q<M.Length, Kilometers, decimal>
```

is a length expressed in kilometers.

```csharp
Q<M.Mul<M.Length, M.Length>, Kilometers, decimal>
```

is a `Length x Length` magnitude expressed on the kilometer basis and is naturally read in `km²`.

```csharp
Q<
    M.Mul<M.Mul<M.Length, M.Length>, M.Length>,
    Kilometers,
    decimal>
```

is naturally read in `km³`.

No `SquaredKilometers`, `CubicKilometers`, or parallel coordinate algebra is required.

## Why `Q` accepts `Coordinate` instead of `Coordinate<F>`

For primitive quantities the compiler can express the strongest relation directly:

```csharp
Length<SELF, C, T>
    where C : Coordinate<M.Length>
```

But composed magnitudes may still be expressed on a primitive basis:

```csharp
Area<Kilometers, T>
    : Q<M.Mul<M.Length, M.Length>, Kilometers, T>
```

`Kilometers` belongs to primitive `M.Length`, not to `M.Mul<M.Length,M.Length>`.

Trying to force `Coordinate<F>` onto every composed magnitude previously required types such as `ProductCoordinate<...>`, which duplicated algebra already present in `M` and created substantial visual noise.

The public core therefore uses:

```csharp
Q<F, C, T>
    where F : M
    where C : Coordinate
```

Concrete semantic types retain stronger coordinate-family constraints wherever C# can express them cleanly. Coherence rules that cannot be represented without distorting the public model are candidates for static analyzer/tooling checks rather than a second coordinate type system.

## Nominal quantities and `SELF`

Nominally closed primitive families place `SELF` first:

```csharp
Length<SELF, C, T>
Mass<SELF, C, T>
Duration<SELF, C, T>
```

`SELF` is the nominal type arithmetic preserves. `C` is the coordinate basis and `T` is the numeric carrier.

For example, cross-coordinate addition remains left-biased:

```text
2 km + 300 m
300 m -> 0.3 km
= 2.3 km
```

The conversion uses:

```text
right.Value
* RIGHT_C.ReferenceScale
/ LEFT_C.ReferenceScale
```

## Structural Product

`Product` is the materialized structural value produced by an authorized multiplication. It is distinct from `M.Mul`, which is only the algebraic magnitude shape.

### One shared coordinate basis

When the result can truthfully be expressed on one primitive basis:

```csharp
Product<LEFT_F, RIGHT_F, C, T>
    : Q<M.Mul<LEFT_F, RIGHT_F>, C, T>
```

For example:

```text
2 km x 300 m
300 m -> 0.3 km
= 0.6 km²
```

materializes as:

```csharp
Product<M.Length, M.Length, Kilometers, decimal>
```

### Independent coordinate bases

Some structural products do not have one truthful coordinate basis:

```text
Mass<Kilograms> x Length<Meters>
```

That case uses:

```csharp
Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>
```

and intentionally does **not** implement `Q<F,C,T>`, because inventing one `C` would misrepresent the value.

This is the current distinction:

```text
compact Product
    one shared primitive coordinate basis exists

full Product
    operand coordinate bases remain independently represented
```

Representability still does not imply operator authority. Operators remain deny-by-default.

## Area

`Length x Length` first produces a structural Product:

```csharp
var structural = width * depth;
```

and only then receives Area semantics explicitly:

```csharp
using static VSlices.Space.Conversions;

var surface = area(structural);
```

The semantic type is:

```csharp
Area<C, T>
    : Q<M.Mul<M.Length, M.Length>, C, T>
```

and is a `DerivedSpace` of:

```csharp
Product<M.Length, M.Length, C, T>
```

So:

```csharp
Area<Kilometers, decimal>
```

is read in `km²` without a squared-coordinate CLR type.

## Volume

`Area x Length` is explicitly authorized. The incoming Length is converted to the Area's primitive Length basis first.

Example:

```text
surface = 0.6 km²
height  = 500 m
500 m -> 0.5 km
0.6 km² x 0.5 km = 0.3 km³
```

Because both operands can be expressed on the kilometer primitive basis, the structural result is compact:

```csharp
Product<
    M.Mul<M.Length, M.Length>,
    M.Length,
    Kilometers,
    decimal>
```

Semantic establishment remains explicit:

```csharp
var capacity = volume(surface * height);
```

and:

```csharp
Volume<C, T>
    : Q<
        M.Mul<M.Mul<M.Length, M.Length>, M.Length>,
        C,
        T>
```

No `CubicKilometers` type is needed.

## Structural meaning vs semantic meaning

These remain intentionally different:

```text
M.Mul<Length,Length>
    algebraic magnitude shape

Product<...>
    materialized structural multiplication

Area<...>
    explicitly established semantic quantity
```

Likewise:

```text
M.Mul<M.Mul<Length,Length>,Length>
    algebraic magnitude shape

Product<...>
    structural value

Volume<...>
    semantic quantity
```

`[AlgebraicSymbol("area")]` and `[AlgebraicSymbol("volume")]` name explicit establishment operations. Tooling may materialize those operations, but must not invent their semantic relations.

## What the model intentionally does not do

- no `SquaredMeters`, `CubicFeet`, or similar generated coordinate backing types;
- no parallel `ProductCoordinate` algebra that duplicates `M`;
- no implicit `Product -> Area` or `Product -> Volume` conversion;
- no universal multiplication merely because `Product` can represent a result;
- no configurable canonical reference coordinate yet;
- no assumption that dimensional/algebraic equivalence implies semantic identity;
- no attempt to reconstruct every semantic coherence rule inside C# generic constraints.

## Current tooling boundary

C# should enforce the relationships it can express honestly. A future analyzer may verify additional static coherence such as an invalid primitive coordinate basis for a declared semantic quantity.

The analyzer is a complement to the type system, not a hidden replacement for the semantic model.

## Next pressure

`M.Div`, `M.Pow`, quotient/power structural values, algebraic normalization/equivalence, and source-generated algebraic symbols remain open production pressures.
