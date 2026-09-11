# Product / Area / Volume review note

This note is intentionally small. It exists to make the current Product experiment easy to review before we promote Area/Volume semantics or source-generation behavior further.

## Production quantity geometry

The production quantity surface is now:

```csharp
Q<F, C, T>
```

where `C : Coordinate<F>` is the effective coordinate. Unit + Prefix are no longer transported as separate generic axes.

The first production Product pressure builds directly on that geometry.

## Structural Product

Production now contains:

```csharp
Dimension.Product<LEFT, RIGHT>
```

and:

```csharp
ProductCoordinate<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C>
    : Coordinate<Dimension.Product<LEFT_F, RIGHT_F>>
```

The coordinate scale is the product of both operand coordinate scales.

The first executable structural quantity is deliberately explicit:

```csharp
Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>
    : Q<
        Dimension.Product<LEFT_F, RIGHT_F>,
        ProductCoordinate<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C>,
        T>
```

This generic shape is evidence, not a final public-design commitment. C# still needs the dimensional families explicitly because it cannot recover them as associated types from `LEFT_C` and `RIGHT_C` alone.

## First authorized multiplication

The first production operator is only:

```text
Length * Length
```

and requires one converged carrier `T`.

For example:

```text
2 km * 3 m

Value:      6
Dimension:  Product<Length, Length>
Coordinate: Product<Kilometers, Meters>
Carrier:    decimal
```

No universal rule exists saying every `Q<A,...> * Q<B,...>` is legal. Product is able to represent a structural multiplication result; availability of a multiplication remains explicit.

## Structural result vs semantic interpretation

The current production step intentionally stops here:

```text
Length * Length
    -> Product<Length, Length, ...>
```

It does **not** yet produce `Area`.

The earlier Modeling probe remains useful for the intended next semantic step:

```text
Product<Length,Length,...>
    -> area(...)
    -> Area

Area * Length
    -> Product<Area,Length,...>
    -> volume(...)
    -> Volume
```

No implicit conversion is intended. The proposed ergonomic surface remains generated static functions imported with `using static`.

## `AlgebraicSymbol`

The earlier probe uses declarations equivalent to:

```csharp
[AlgebraicSymbol("area")]
Area ...

[AlgebraicSymbol("volume")]
Volume ...
```

The attribute does not define the algebraic relationship itself. That relationship must already be present in the semantic model.

Its intended purpose is to authorize an explicit establishment function that Tooling may generate:

```csharp
public static Area area(Product<...> value);
public static Volume volume(Product<...> value);
```

Current working rule:

> Source generation may complete a declared semantic relation; it must not invent the relation.

## Generated ownership surface

The intended split remains:

```text
VSlices-owned declarations
    -> VSlices `Conversions`

consumer-owned declarations
    -> project-configured `CustomConversions`
```

The consumer pays one configuration cost per project and then imports the generated surface through `using static`.

## Things worth challenging during review

1. Does `Product<LEFT_F, LEFT_C, RIGHT_F, RIGHT_C, T>` expose too much realization detail, or is that unavoidable until Tooling can hide/recover it?
2. Should Product keep operand dimensions/coordinates explicitly, or should only the resulting `Q<F,C,T>` shape survive?
3. Does `Dimension.Product<A,B>` deserve CLR identity exactly in operand order, or will algebraic equivalence later need a separate normalization relation?
4. Is `ProductCoordinate` correctly structural, including non-normalized forms such as `Kilometers × Meters`?
5. Is requiring a common `T` the right default, leaving mixed-carrier multiplication to an explicit policy?
6. What declaration should authorize `Length * Length`: an explicit operation declaration, a semantic target such as Area, or another mechanism?
7. Can `Area` later remain semantic in `Product<Area,Length,...>` without forcing immediate expansion to `Product<Product<Length,Length>,Length,...>`?

## Current files

Production pressure:

```text
src/VSlices.Space/Quantities/Quantity.cs
src/VSlices.Space/Quantities/Product.cs
src/VSlices.Space/Quantities/Length.cs
tests/VSlices.Space.Tests/ProductTests.cs
```

Earlier semantic-establishment probe:

```text
examples/VSlices.Space.Modeling/AlgebraicSymbolAreaVolumeProbe.cs
tests/VSlices.Space.Modeling.Tests/AlgebraicSymbolAreaVolumeProbeTests.cs
```

The next boundary is `Product -> area(...) -> Area` against the real production `Q<F,C,T>` surface.
