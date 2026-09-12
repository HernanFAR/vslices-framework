# Quantities

This folder contains the current quantity model used by `VSlices.Space`.

The model currently separates:

```text
M            algebraic magnitude shape
Coordinate   primitive measurement basis
T            numeric carrier
```

The current quantity abstraction is:

```csharp
Q<F, C, T>
```

where `F : M`, `C : Coordinate`, and `T` is the numeric carrier.

A design question is intentionally open:

> **Is `Q<F,C,T>` actually the universal shape of a quantity, or only one useful quantity shape?**

`M.Div` shows a multi-basis quantity that does not fit one `C`; `M.Pow` now pressures the opposite case, where repeated algebraic structure still uses one primitive basis naturally.

The guiding rule remains:

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

These types describe algebraic shape only. They do not by themselves authorize operations or establish semantic quantities such as Area, Volume, Speed, Energy, or Torque.

Examples:

```csharp
M.Mul<M.Length, M.Length>
M.Div<M.Length, M.Duration>
M.Pow<M.Length, N2>
```

respectively describe `Length x Length`, `Length / Duration`, and `Length²` algebraic shapes.

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
Hours      : Coordinate<M.Duration>
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
Hours.ReferenceScale      = 3600
```

The reference coordinate is currently a VSlices convention, not a customizable setting.

The reference coordinate is also distinct from a preferred/default coordinate. Recommended `Mass<T>`, for example, uses Kilograms while Mass reference scaling remains relative to Grams.

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

is a `Length x Length` magnitude expressed on the kilometer primitive basis.

```csharp
Q<M.Pow<M.Length, N2>, Kilometers, decimal>
```

is a `Length²` magnitude expressed on that same primitive kilometer basis.

No `SquaredKilometers`, `CubicKilometers`, or parallel coordinate algebra is required.

## `Q<F,C,T>` and its current boundary

For primitive and single-basis composed quantities, `Q` remains useful:

```csharp
Q<F, C, T>
    where F : M
    where C : Coordinate
```

Primitive semantic families retain stronger constraints where C# can express them honestly:

```csharp
Length<SELF, C, T>
    where C : Coordinate<M.Length>
```

The important limitation exposed by division is that some quantities naturally contain more than one primitive coordinate basis. `km/h` genuinely contains both Length and Duration bases, so forcing it into one synthetic `C` would recreate coordinate algebra we deliberately removed.

Power provides the complementary evidence: `km²` produced from one Length basis can still truthfully use `Kilometers` as its single primitive `C`, because the exponent belongs to the magnitude shape rather than the coordinate.

The current hypothesis is therefore:

```text
Q<F,C,T> is a useful quantity shape for values with one truthful primitive basis.
```

Whether `Q` should survive as a named abstraction, be generalized, or be replaced remains deliberately unresolved.

## Nominal quantities and `SELF`

Nominally closed primitive families place `SELF` first:

```csharp
Length<SELF, C, T>
Mass<SELF, C, T>
Duration<SELF, C, T>
```

`SELF` is the nominal type arithmetic preserves. `C` is the coordinate basis and `T` is the numeric carrier.

## Structural Product

`Product` is the materialized structural result of an authorized multiplication. It is distinct from `M.Mul`, which is only the algebraic magnitude shape.

When one primitive basis truthfully expresses the result:

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

When no single truthful basis exists, the full Product preserves both independently:

```csharp
Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>
```

and intentionally does not implement `Q<F,C,T>`.

## Structural Quotient

`M.Div` is pressured by:

```text
Length / Duration
```

The authorized operator materializes:

```csharp
Quotient<
    M.Length,
    LENGTH_C,
    M.Duration,
    DURATION_C,
    T>
```

For example:

```text
120 km / 2 h = 60 km/h
```

The Quotient preserves both primitive coordinate bases and does **not** implement `Q<F,C,T>`.

## Structural Power

`M.Pow` is now pressured by the real case of squaring a Length.

Exponent vocabulary is introduced only as needed; the first exponent is:

```csharp
N2
```

and the structural value is:

```csharp
Power<BASE_F, EXPONENT, C, T>
    : Q<M.Pow<BASE_F, EXPONENT>, C, T>
```

For example:

```csharp
using static VSlices.Space.Quantities.PowerOperations;

var side = new Length<decimal>(3m);
var structural = square(side);
```

conceptually yields:

```text
3 m -> 9 m²
```

with the type:

```csharp
Power<M.Length, N2, Meters, decimal>
```

The coordinate remains `Meters`; the square lives entirely in `M.Pow<M.Length,N2>`.

This pressure also makes an algebraic-equivalence question concrete:

```csharp
M.Pow<M.Length, N2>
```

and:

```csharp
M.Mul<M.Length, M.Length>
```

may describe equivalent dimensional structure, but they are intentionally still distinct CLR/algebraic forms. No normalization or equivalence rule has been introduced yet.

Likewise, `Power<M.Length,N2,...>` is not automatically established as `Area`. Doing so would require deciding how semantic Area relates to multiple structural representations (`Mul` and `Pow`), which is a separate pressure rather than something Power should silently decide.

## Area and Volume

Area is explicitly established from structural `Length x Length`:

```csharp
var surface = area(width * depth);
```

with:

```csharp
Area<C, T>
    : Q<M.Mul<M.Length, M.Length>, C, T>
```

Volume is explicitly established from `Area x Length` after aligning the incoming Length to Area's primitive Length basis:

```csharp
var capacity = volume(surface * height);
```

with:

```csharp
Volume<C, T>
    : Q<
        M.Mul<M.Mul<M.Length, M.Length>, M.Length>,
        C,
        T>
```

Both remain single-basis quantities, so the current `Q` shape fits them naturally.

## Speed

Speed semantics are established explicitly:

```csharp
var structural = distance / duration;
var semanticSpeed = speed(structural);
```

The semantic type is:

```csharp
Speed<LENGTH_C, DURATION_C, T>
```

and is a `DerivedSpace` of the corresponding structural Quotient.

Crucially, Speed does **not** implement `Q<F,C,T>` because doing so would require inventing one coordinate parameter where two primitive bases are semantically real.

## Structural meaning vs semantic meaning

The separation remains:

```text
M.Mul<Length,Length>
    algebraic magnitude shape
Product<...>
    materialized multiplication
Area<...>
    established semantic quantity
```

```text
M.Div<Length,Duration>
    algebraic magnitude shape
Quotient<...>
    materialized division
Speed<...>
    established semantic quantity
```

and Power currently stops deliberately at the structural layer:

```text
M.Pow<Length,N2>
    algebraic magnitude shape
Power<...>
    materialized exponentiation
```

No semantic interpretation is inferred merely because a structural value exists.

## Tooling boundary

C# should enforce the relationships it can express honestly. A future analyzer may verify additional semantic coherence that cannot be represented without distorting the public model.

The analyzer is a complement to the type system, not a hidden replacement for it.

## Current open questions

- Is `Q<F,C,T>` a useful specialized single-basis shape, or should it evolve into a more general quantity abstraction?
- Should multi-basis semantic quantities share a common abstraction with single-basis quantities at all?
- Should quotient coordinates later support explicit conversion as a pair?
- What algebraic equivalence, if any, should relate `M.Pow<X,N2>` and `M.Mul<X,X>`?
- Should semantic Area eventually accept both Product and Power structural representations, and if so what owns their normalization?
- Source-generated algebraic symbols remain later work.
