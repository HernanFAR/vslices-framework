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

A new design question is now intentionally open:

> **Is `Q<F,C,T>` actually the universal shape of a quantity, or only one useful quantity shape?**

`M.Div` is the first production pressure that makes this question concrete.

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

These types describe algebraic shape only. They do not by themselves authorize operators or establish semantic quantities such as Area, Volume, Speed, Energy, or Torque.

For example:

```csharp
M.Mul<M.Length, M.Length>
```

means `Length x Length`; it does not automatically mean Area.

Likewise:

```csharp
M.Div<M.Length, M.Duration>
```

means `Length / Duration`; it does not automatically mean Speed.

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

is a `Length x Length` magnitude expressed on the kilometer primitive basis and is naturally read in `km²`.

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

and:

```csharp
Area<C, T>
    where C : Coordinate<M.Length>
```

The important limitation is now visible: some quantities are naturally expressed by **more than one primitive coordinate basis**.

For example:

```text
km/h
```

contains both a Length basis and a Duration basis. Inventing one synthetic `C` would recreate the coordinate algebra the model deliberately removed.

So the current hypothesis is no longer:

```text
all quantities are Q<F,C,T>
```

but rather:

```text
Q<F,C,T> is one quantity shape for values with one truthful primitive basis.
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

## Structural Quotient

`M.Div` is now pressured by the real case:

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

materializes as:

```csharp
Quotient<
    M.Length,
    Kilometers,
    M.Duration,
    Hours,
    decimal>
```

The structural Quotient deliberately preserves both primitive coordinate bases and does **not** implement `Q<F,C,T>`.

This is not an implementation workaround. `km/h` genuinely does not have one truthful primitive `C`.

The quotient also preserves the chosen coordinate pair instead of silently normalizing it:

```text
120 km / 2 h   = 60 km/h
120 km / 120 m = 1 km/min
```

Both values describe the same physical rate after conversion, but they are expressed in different coordinate pairs.

## Speed

Speed semantics are established explicitly:

```csharp
var velocityMagnitude = distance / duration;
var semanticSpeed = speed(velocityMagnitude);
```

The semantic type is currently:

```csharp
Speed<LENGTH_C, DURATION_C, T>
```

and is a `DerivedSpace` of:

```csharp
Quotient<
    M.Length,
    LENGTH_C,
    M.Duration,
    DURATION_C,
    T>
```

`Speed<Kilometers,Hours,T>` is therefore naturally read as km/h.

Crucially, Speed currently does **not** implement `Q<F,C,T>` because doing so would require inventing one coordinate parameter where two primitive bases are semantically real.

That makes Speed the first direct evidence that `Q<F,C,T>` may not be the universal quantity abstraction.

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

and now:

```text
M.Div<Length,Duration>
    algebraic magnitude shape

Quotient<...>
    materialized division

Speed<...>
    established semantic quantity
```

`[AlgebraicSymbol("area")]`, `[AlgebraicSymbol("volume")]`, and `[AlgebraicSymbol("speed")]` name explicit establishment operations. Tooling may materialize those operations, but must not invent their semantic relations.

## Tooling boundary

C# should enforce the relationships it can express honestly. A future analyzer may verify additional semantic coherence that cannot be represented without distorting the public model.

The analyzer is a complement to the type system, not a hidden replacement for it.

## Current open questions

- Is `Q<F,C,T>` a useful specialized shape, or should it evolve into a more general quantity abstraction?
- Should multi-basis semantic quantities share a common abstraction with single-basis quantities at all?
- Should quotient coordinates later support explicit conversion as a pair, rather than normalization into one synthetic coordinate?
- `M.Pow`, structural Power values, algebraic normalization/equivalence, and source-generated algebraic symbols remain later pressures.
