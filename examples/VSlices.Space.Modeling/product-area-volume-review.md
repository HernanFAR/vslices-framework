# Product -> Area review

This note records the current production pressure over `Q<F,C,T>`.

## Quantity basis

Production uses:

```csharp
Q<F,C,T>
```

`F` carries dimensional meaning, `C` carries the coordinate basis used to express that family, and `T` carries the numeric representation.

A key consequence is that algebraic shape should not be duplicated inside the coordinate type. We should not need types such as `SquaredMeters`, `SquaredKilometers`, or `CubedFeet` merely because the dimensional expression is squared or cubed. The dimensional expression already carries that information.

## Two structural Product forms

Two canonical Product shapes are now under pressure.

### Homogeneous / coordinate-converged Product

When both operands belong to a dimensional family whose coordinates can be converted into one another, the right operand is converted to the left coordinate before multiplication.

```text
Q<F,C1,T> * Q<F,C2,T>
    -> convert C2 to C1
    -> Product<F,F,C1,T>
```

`C1` is not a generated square-coordinate type. It remains the base coordinate used to express the product.

Example:

```text
2 km * 3 m
3 m -> 0.003 km
2 * 0.003 -> 0.006

Product<Length,Length,Kilometers,decimal>
```

The dimensional family `Product<Length,Length>` supplies the square shape, so the value is understood as `0.006 km²` without requiring a `SquaredKilometers` CLR type.

The corresponding `Q` membership uses a structural adapter:

```csharp
Q<
    Dimension.Product<Length,Length>,
    ProductCoordinate<Length,Length,Kilometers>,
    decimal>
```

`ProductCoordinate<LEFT_F,RIGHT_F,C>` lifts the shared coordinate basis into the composed dimension. Its scale is `C.Scale * C.Scale`.

### Heterogeneous Product

When dimensions are different, there is no meaningful coordinate conversion such as meters -> kilograms. Both coordinate bases must therefore remain visible:

```csharp
Product<LEFT_F,LEFT_C,RIGHT_F,RIGHT_C,T>
```

with membership:

```csharp
Q<
    Dimension.Product<LEFT_F,RIGHT_F>,
    ProductCoordinate<LEFT_F,LEFT_C,RIGHT_F,RIGHT_C>,
    T>
```

Example:

```text
Product<Mass,Kilograms,Length,Meters,decimal>
```

represents a structural `kg*m` quantity without inventing domain semantics.

## Area

Area now uses the homogeneous Product directly:

```csharp
[AlgebraicSymbol("area")]
Area<C,T>
    : Q<
        Dimension.Product<Length,Length>,
        ProductCoordinate<Length,Length,C>,
        T>
    : DerivedSpace<
        Area<C,T>,
        Product<Length,Length,C,T>>
```

Consumption remains explicit:

```csharp
using static VSlices.Space.Quantities.Conversions;

var structural = width * depth;
var semantic = area(structural);
```

This keeps three statements separate:

```text
Length * Length
    authorizes and materializes a structural Product

Product<Length,Length,C,T>
    carries the algebraic dimensional shape and coordinate basis

area(...)
    explicitly establishes Area semantics
```

No implicit conversion is introduced.

## Current interpretation

```text
Dimension tells us what is composed.
Coordinate tells us how the basis is measured.
Product realization combines both without manufacturing one coordinate type per algebraic operation.
```

The homogeneous/heterogeneous split is not merely ergonomic. It records whether operand coordinates admit convergence to one shared basis.

## Open pressure

The next case should be `Area * Length -> Product -> volume(...) -> Volume`. That case should tell us whether the homogeneous Product shape generalizes cleanly when one operand already carries semantic information, and whether `Area` should remain visible as an operand or expose its structural base during composition.
