# VSlices.Space

`VSlices.Space` is the semantic surface for describing what can validly exist and the structures recognized over those values.

This project is being extracted incrementally from `VSlices.Domain`. The migration is intentionally not one-to-one: concepts are moved only when their independent semantics are clear, while legacy composites and representation-coupled abstractions are reconsidered instead of mechanically preserved.

## Current scope

The current space-level concepts are:

- `DiscreteSpace`: a semantic space whose values support an explicitly defined notion of equality.
- `VectorSpace`: a discrete space with vector-space operations over a scalar type.
- `AffineSpace`: a discrete space whose points can be translated by vectors and whose subtraction yields a displacement.
- `MaintainedSpace`: a discrete space whose complete recognized set of values is explicitly maintained.
- `DerivedSpace`: a semantic subset of another discrete space, with total semantics-preserving widening to its base space.

The semantic laws governing `DerivedSpace` are centralized in `DerivedSpaceLaws`.

## Direction of the migration

`VSlices.Domain` is kept temporarily while concepts are migrated in small, verifiable slices. During this transition it may depend on `VSlices.Space`.

Point/value characteristics such as transformation, validation and evolution are expected to be evaluated separately from traits that describe spaces themselves.

Common semantic values such as `Length`, `Mass`, `Duration`, `Money` and related concepts are expected to remain first-class values rather than collapse into aliases of dimensional expressions. Two values can share dimensional structure while still represent different semantic concepts.

## Under review

`QuantitySpace` is intentionally not migrated yet. Its current `CanonValue` requirement is under semantic review: the open question is whether a canonical scalar coordinate belongs to the contract of the space or whether unit/carrier conversion belongs primarily to grounding or concrete realization.

The pending dimensional composition work (`Power`, `Product`, `Quotient`, and related structures) remains outside the current migration slice until `QuantitySpace` is clarified.

## Working criterion

A concept belongs in this project when it describes a semantic space, a relation between semantic spaces, or laws intrinsic to those structures. Composite interfaces must add independent semantic meaning; reducing typing alone is not sufficient justification.
