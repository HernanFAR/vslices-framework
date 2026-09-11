# Product / Area / Volume review note

This note is intentionally small. It exists to make the current Product experiment easy to review before we promote any of it into the production `VSlices.Space` surface.

## What the experiment is trying to prove

We currently want multiplication to be **available only when an explicit structural operation has been declared**.

The type system must not imply a universal rule such as:

```text
A * B -> Product<A,B>
```

for every pair of quantity spaces.

Instead, the current pressure case authorizes exactly these two operations:

```text
Length * Length
    -> Product<Length,Length,...>

Area * Length
    -> Product<Area,Length,...>
```

and deliberately leaves operations such as:

```text
Area * Area
```

undefined.

The negative-compilation probe verifies that this unavailable operation stays unavailable.

## Structural result vs semantic interpretation

`Product` is currently being pressured as a canonical **structural** multiplication result.

```text
Length * Length
    -> Product<Length,Length,...>
```

This does **not** mean that the result is already `Area`.

The semantic interpretation is explicit:

```text
Product<Length,Length,...>
    -> area(...)
    -> Area
```

Likewise:

```text
Area * Length
    -> Product<Area,Length,...>
    -> volume(...)
    -> Volume
```

The desired C# consumption surface is therefore:

```csharp
using static ...Conversions;

var areaValue = area(width * depth);
var volumeValue = volume(areaValue * height);
```

No implicit conversion is intended.

## Why `Area` is preserved inside the next Product

The second multiplication intentionally produces:

```text
Product<Area,Length,...>
```

rather than immediately expanding Area back into:

```text
Product<Product<Length,Length,...>,Length,...>
```

The first form preserves knowledge already established by the model: the operand is an `Area`.

Whether these two structural forms are algebraically compatible is a separate problem:

```text
Product<Area,Length,...>

vs

Product<Product<Length,Length,...>,Length,...>
```

They are not the same CLR type. Future work may establish algebraic equivalence or expansion without erasing their distinct syntactic/semantic forms.

## `AlgebraicSymbol`

The current probe uses declarations equivalent to:

```csharp
[AlgebraicSymbol("area")]
Area ...

[AlgebraicSymbol("volume")]
Volume ...
```

The attribute does not define the algebraic relationship itself. That relationship must already be present in the type declaration / semantic model.

Its purpose is to authorize the explicit establishment function that Tooling may generate:

```csharp
public static Area area(Product<Length,Length,...> value);
public static Volume volume(Product<Area,Length,...> value);
```

Current working rule:

> Source generation may complete a declared semantic relation; it must not invent the relation.

## Generated ownership surface

The intended split is:

```text
VSlices-owned declarations
    -> VSlices `Conversions`

consumer-owned declarations
    -> project `CustomConversions`
```

For consumer-owned conversions, the project pays a one-time configuration cost for the target namespace/class and then imports it through `using static`.

This keeps VSlices-generated API separate from application/domain-generated API.

## Current executable Product shape

The conceptual notation used during discussion is:

```text
Product<A,B>
```

The first executable probe currently needs:

```csharp
Product<A,B,T>
```

because the resulting value needs a converged numeric carrier.

This is **not yet a production decision**.

The useful question is whether `T`:

- genuinely belongs to `Product`;
- can be recovered from its operands;
- belongs to a common quantity carrier abstraction instead;
- disappears once the real `QuantitySpace` representation is used.

The probe should be treated as evidence, not as the final generic signature.

## Things worth challenging during review

1. Does `Product` deserve to be a real quantity-space type, or is it merely structural metadata around another quantity representation?
2. Is preserving `Area` in `Product<Area,Length,...>` the right default, or should composition operate on expanded structural bases?
3. Does `DerivedSpace<Area,Product<Length,Length,...>>` truthfully express the relationship we want, or do algebraic semantic interpretations need a distinct relation?
4. Is the carrier `T` genuinely part of Product identity, or only realization state?
5. Which declaration should authorize `Length * Length`: the Product structure itself, `Area`, or an explicit operation declaration separate from both?
6. Can a source generator derive the exact C# operator surface from the semantic declarations without inventing additional operations?
7. What diagnostic should be produced when two `[AlgebraicSymbol]` declarations would generate the same invalid/ambiguous C# member?

## Current files

The executable probe is:

```text
examples/VSlices.Space.Modeling/AlgebraicSymbolAreaVolumeProbe.cs
```

Its behavioral tests are:

```text
tests/VSlices.Space.Modeling.Tests/AlgebraicSymbolAreaVolumeProbeTests.cs
```

The negative-compilation path is exercised through the existing `MODELING_INVALID_USAGE` build in Space Modeling CI.

## Current evidence

At the time this note was added, Space Modeling CI #81 had passed the Area / Volume pressure case.

That establishes only that the current C# 14 surface is realizable and that undeclared `Area * Area` multiplication remains unavailable. It does **not** establish that this is the final Product design.
