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

`QuantityFamilies/QuantityFamily.cs` explores a different geometry:

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

The generic argument order is conceptual rather than selected for defaulting convenience. Unit currently forms a stronger neighborhood than Prefix, and Prefix a stronger neighborhood than numeric carrier. Therefore:

```text
Mass<Grams, None, decimal>
```

is considered conceptually closer to:

```text
Mass<Grams, Micro, double>
```

than to:

```text
Mass<Pounds, None, decimal>
```

The recommended specialization ladder currently is:

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

The general Mass family owns exact-coordinate arithmetic and explicit cross-coordinate conversion. Cross-coordinate conversion is currently left-biased:

```text
Mass<U1,P1,T1> + Mass<U2,P2,T2>
    -> Mass<U1,P1,T1>
```

The right quantity is converted into the left coordinate through:

```text
Unit scale
+ Prefix scale
+ T1.CreateChecked(right.Value)
```

This means Unit, Prefix, and .NET generic math each own one part of the realization rather than Mass containing a catalog of concrete conversions.

### C# 14 extension-operator pressure

The first version of this probe concluded that a fully open operator was blocked because ordinary C# operator declarations cannot introduce their own generic parameters. That conclusion was incomplete for the .NET 10 / C# 14 target.

C# 14 extension blocks can introduce generic parameters that are inferred from the combined extension receiver and operator operands. The probe now declares an extension block whose six generic parameters describe both Mass operands:

```csharp
extension<LU, LP, LT, RU, RP, RT>(Mass<LU, LP, LT>)
{
    public static Mass<LU, LP, LT> operator +(
        Mass<LU, LP, LT> left,
        Mass<RU, RP, RT> right);
}
```

with dimensional, unit, prefix, and numeric constraints on all parameters.

This directly tests whether natural operator syntax can survive across the full same-dimension family:

```text
Mass<Grams,Kilo,double>
+
Mass<Grams,Micro,decimal>

Mass<Grams,None,decimal>
+
Mass<Pounds,None,double>
```

The semantic policy has not changed: the result remains left-biased and numeric conversion remains checked. What changed is the C# realization mechanism available to express that policy.

If the compiler accepts this shape, operator availability no longer needs to decrease merely because Unit, Prefix, or carrier differ. The remaining design question becomes semantic and ergonomic: at what representational distance should VSlices still prefer an explicit operation because an operator would hide too much policy?

## Current questions

The quantity-family experiment is still pressure, not a settled public API. Immediate questions include:

- whether `Q<F,U,P,T>` adds genuine quantity-family semantics rather than becoming a renamed generic-math mechanism;
- which capabilities belong to `Q` versus concrete quantity families;
- whether C# 14 extension operators remain usable with recommended descendants and consumer-defined Mass realizations;
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
