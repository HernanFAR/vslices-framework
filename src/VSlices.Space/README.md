# VSlices.Space

`VSlices.Space` is the semantic surface for describing what values can validly exist and which structures are intrinsic to those values.

Its current working principle is:

> Space establishes meaning. Grounding establishes contact.

The abstractions in this project are intentionally pressure-tested from real cases. They should not gain operations merely because their CLR representation supports those operations.

## Core spaces

- `DiscreteSpace<SELF>` expresses semantic equality without introducing ordering or algebra.
- `VectorSpace<SELF,SCALAR>` expresses additive vector structure and scalar multiplication/division.
- `AffineSpace<POINT,DISTANCE,SCALAR>` separates points from the vectors that translate them.
- `MaintainedSpace<SELF>` expresses a complete recognized set maintained explicitly by the type.
- `DerivedSpace<SELF,BASE>` expresses semantic subset inclusion (`SELF ⊆ BASE`) and total widening to the base space.

## Quantity families

The current quantitative surface lives under `VSlices.Space.Quantities`.

```text
Q<F,U,P,T>
```

expresses quantity-family membership together with a representation coordinate:

```text
F = dimensional family
U = Unit
P = Prefix
T = numeric carrier
```

The first concrete dimensional families are:

```text
Mass
Length
Duration
```

They intentionally provide three independent base dimensions to pressure dimensional composition without prematurely introducing a complete physical-units algebra.

Current recommended forms are:

```text
vMass     = kilograms over double
vLength   = meters over double
vDuration = seconds over double
```

Each family also exposes progressively more open generic forms so consumers and Tooling can choose unit, prefix, carrier, and nominal closure where necessary.

### Nominal closure

The most general family types include `SELF`:

```text
Mass<U,P,T,SELF>
Length<U,P,T,SELF>
Duration<U,P,T,SELF>
```

`SELF` means that homogeneous arithmetic closes over the nominal type of the left operand.

The runtime reconstruction step uses LanguageExt's cached constructor delegate mechanism (`IL.Ctor<T,SELF>()`). The required `SELF(T value)` constructor shape is therefore a realization convention rather than additional semantic vocabulary. VSlices Tooling is expected to generate or verify that shape for generated Values.

### Coordinate conversion

Homogeneous addition and subtraction currently use a deterministic left-biased policy:

```text
right quantity
  -> right Unit/Prefix scale
  -> left carrier via checked numeric conversion
  -> left Unit/Prefix coordinate
  -> arithmetic
  -> left nominal SELF
```

The policy is deliberately still under pressure for representability, rounding, and overflow.

## Next pressure: dimensional composition

`Mass`, `Length`, and `Duration` now form the minimal independent pressure set for discovering the semantics of:

```text
Product<A,B>
Quotient<N,D>
Power<B,E>
```

Representative cases include:

```text
Mass * Length
Length * Length
Length / Duration
Mass / Duration
Length / Length
Length^2
```

These structural dimensional constructions must remain distinct from domain semantic names. Two domain quantities may share dimensional geometry without therefore being the same semantic Space.

## Working criteria

- Semantic structure precedes realization convenience.
- A composite abstraction must add independent meaning, laws, or authority.
- Do not reconstruct guarantees already expressed by semantic types as business invariants.
- Carrier operations do not automatically belong to the semantic Space.
- Generated realization constraints should remain owned by Tooling when they do not constitute semantic vocabulary.
