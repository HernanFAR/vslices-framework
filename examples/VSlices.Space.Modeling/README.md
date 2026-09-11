# VSlices.Space.Modeling

This project is a modeling probe for the emerging `VSlices.Space` API. It is intentionally not an automated test project.

Its job is to make semantic decisions executable enough that their C# consequences can be inspected before those decisions become stable framework contracts.

## Location probe

`Location` composes independently established semantic values and persistent evolution. `LocationName` is established through `Transformable<string, LocationName>`; `Location.Input` and `Location.State` reuse that semantic type rather than duplicating its invariants. `Location.State` keeps `Name` creation-fixed while `X/Y` are proposal-updatable. The private State constructor is bridged only by a private .NET `UnsafeAccessor`, which is treated as a realization mechanism rather than semantic authority.

The negative-compilation probe in `InvalidUsage.cs` verifies that callers cannot mint semantic state, bypass `LocationName` establishment, replace a creation-fixed name, or replace accepted state directly.

## Quantity experiments

Two quantity shapes intentionally coexist so they can pressure each other rather than forcing an early migration.

### Original structural quantity probe

`Quantities/` currently contains the earlier `Quantity<DIM, PREFIX>` experiment and provisional `vMass` Value. It established canonical-coordinate, prefix-conversion, vector-operation, and negative-mass pressure, but an instantiable generic structural Quantity may be mixing quantity-family structure with concrete Value identity.

### Q quantity-family probe

`QuantityFamilies/QuantityFamily.cs` explores:

```text
Q<F, U, P, T>

F = dimensional family
U = Unit
P = Prefix
T = backing numeric type
```

with:

```csharp
where F : Dimension
where U : Unit<F>
where P : Prefix
where T : INumberBase<T>
```

Concrete `Mass<U,P,T>` strengthens the carrier to `INumber<T>` because it owns arithmetic.

The generic argument order is conceptual: Unit currently forms a stronger neighborhood than Prefix, and Prefix a stronger neighborhood than numeric carrier. `Mass<Grams,None,decimal>` is therefore considered conceptually closer to `Mass<Grams,Micro,double>` than to `Mass<Pounds,None,decimal>`.

The recommended specialization ladder is:

```text
abstract Mass<U,P,T>
    consumer chooses Unit + Prefix + carrier

abstract Mass<P,T>
    = Mass<Grams,P,T>

Mass<T>
    = Mass<Grams,Kilo,T>

vMass
    = Mass<Grams,Kilo,double>
```

These forms progressively fix VSlices defaults rather than introduce new quantity semantics.

## Quantity-family interoperability

The general Mass family owns exact-coordinate arithmetic and cross-coordinate conversion. Conversion is currently left-biased:

```text
Mass<U1,P1,T1> + Mass<U2,P2,T2>
    -> Mass<U1,P1,T1>
```

The right quantity is converted into the left coordinate through Unit scale, Prefix scale, and `T1.CreateChecked(right.Value)`. Unit, Prefix, and .NET generic math therefore each own one part of the realization rather than Mass containing a catalog of concrete conversions.

### C# 14 extension-operator pressure

The earlier conclusion that fully open operator syntax was blocked by C# was incomplete. On the current `.NET 10` target, C# 14 extension blocks can introduce generic parameters inferred from the combined extension receiver and operator operands.

The probe now uses a six-parameter extension block:

```csharp
extension<LU, LP, LT, RU, RP, RT>(Mass<LU, LP, LT>)
{
    public static Mass<LU, LP, LT> operator +(
        Mass<LU, LP, LT> left,
        Mass<RU, RP, RT> right);
}
```

with corresponding Unit, Prefix, Dimension, and generic-math constraints. The semantic operation remains implemented by `Mass.Add` / `Mass.Subtract`; the extension block only supplies operator realization.

Behavioral tests pressure normal `+` / `-` syntax for same coordinates, different Prefix/carrier under one Unit, fully different Unit/Prefix/carrier within Mass, recommended `Mass<T>` descendants, and `vMass`.

The semantic policy has not changed: conversion remains checked and the result remains left-biased. What changed is the C# realization mechanism available to express it.

The remaining question is semantic and ergonomic rather than merely syntactic: when does an operator hide too much conversion policy, even if C# can express and infer it?

## Current questions

The quantity-family experiment remains pressure, not a settled public API. Immediate questions include:

- whether `Q<F,U,P,T>` adds genuine quantity-family semantics rather than becoming a renamed generic-math mechanism;
- which capabilities belong to `Q` versus concrete quantity families;
- whether C# 14 extension-operator inference remains usable with recommended descendants and consumer-defined realizations;
- whether static result types preserve enough nominal information across operations;
- how Transformable should participate in quantity establishment after the family geometry stabilizes;
- what role, if any, remains for the older `Quantity<DIM,PREFIX>` structural type;
- and only later, how dimensions compose through Product, Quotient, and Power.

## Verification

Normal build:

```text
dotnet build examples/VSlices.Space.Modeling/VSlices.Space.Modeling.csproj
```

Behavioral tests:

```text
dotnet test tests/VSlices.Space.Modeling.Tests/VSlices.Space.Modeling.Tests.csproj
```

Negative compile pressure:

```text
dotnet build examples/VSlices.Space.Modeling/VSlices.Space.Modeling.csproj -p:DefineConstants=MODELING_INVALID_USAGE
```

The last command is expected to fail. `.github/workflows/space-modeling.yml` exercises all three paths on pull requests.
