# VSlices.Space

`VSlices.Space` is the semantic surface for describing what can validly exist, the structures recognized over those values, relations between semantic spaces, and characteristics intrinsic to points inhabiting them.

This project is being extracted incrementally from `VSlices.Domain`. The migration is intentionally not one-to-one: concepts are moved only when their independent semantics are clear, while legacy composites and representation-coupled abstractions are reconsidered instead of mechanically preserved.

## Current scope

The current space-level concepts are:

- `DiscreteSpace`: a semantic space whose values support an explicitly defined notion of equality.
- `VectorSpace`: a discrete space with vector-space operations over a scalar type.
- `AffineSpace`: a discrete space whose points can be translated by vectors and whose subtraction yields a displacement.
- `MaintainedSpace`: a discrete space whose complete recognized set of values is explicitly maintained.
- `DerivedSpace`: a semantic subset of another discrete space, with total semantics-preserving widening to its base space.
- `Transformable`: a potentially fallible semantic transformation relation between a source and target space, owned by an explicit semantic context or by the target itself.

Point-level characteristics currently include:

- `Evolvable`: an existing point can propose a change over its current state and, when accepted, produce a new valid instance without mutating the source point.

The semantic laws governing `DerivedSpace` are centralized in `DerivedSpaceLaws`.

## Input and state

The current modeling direction distinguishes two surfaces that concrete semantic types may expose:

```text
Input = semantic knowledge required to attempt initial establishment
State = currently accepted state of an existing point
```

`Input` is not an external representation and may itself contain already-established values from other spaces. Representation conversion remains a Grounding concern.

`State` is not required to have the same shape as `Input`. For evolvable points, construction of an accepted `State` remains controlled by the owning type, while callers may derive candidate states from an already-obtained valid state.

## Direction of the migration

`VSlices.Domain` is kept temporarily while concepts are migrated in small, verifiable slices. During this transition it may depend on `VSlices.Space`.

`Validatable` is currently classified as contextual admissibility owned by Work rather than Space: an already-valid value may or may not be usable in a particular work context.

Common semantic values such as `Length`, `Mass`, `Duration`, `Money` and related concepts are expected to remain first-class values rather than collapse into aliases of dimensional expressions. Two values can share dimensional structure while still represent different semantic concepts.

## Modeling probes

`examples/VSlices.Space.Modeling` exists to exercise emerging contracts as C# models before treating them as stable framework design. It is not an automated test suite. The project contains positive usage examples and opt-in negative compilation probes for restrictions that should be enforced by the type model itself.

## Under review

`QuantitySpace` is intentionally not migrated yet. Its current `CanonValue` requirement is under semantic review: the open question is whether a canonical scalar coordinate belongs to the contract of the space or whether unit/carrier conversion belongs primarily to grounding or concrete realization.

The pending dimensional composition work (`Power`, `Product`, `Quotient`, and related structures) remains outside the current migration slice until `QuantitySpace` is clarified.

## Working criterion

A concept belongs in this project when it describes a semantic space, a relation between semantic spaces, a characteristic intrinsic to points in a space, or laws intrinsic to those structures. Composite interfaces must add independent semantic meaning; reducing typing alone is not sufficient justification.
