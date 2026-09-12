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

`QuantityFamilies/QuantityFamily.cs` explores `Q<F,U,P,T>` where `F` is the dimensional family, `U` the Unit, `P` the Prefix, and `T` the backing numeric type. `Q` constrains `T` only to `INumberBase<T>`; concrete `Mass<U,P,T>` strengthens that to `INumber<T>` because it owns arithmetic.

The generic argument order is conceptual: Unit currently forms a stronger neighborhood than Prefix, and Prefix a stronger neighborhood than numeric carrier. `Mass<Grams,None,decimal>` is therefore considered conceptually closer to `Mass<Grams,Micro,double>` than to `Mass<Pounds,None,decimal>`.

Recommended specialization currently progresses from `Mass<U,P,T>` to `Mass<P,T> = Mass<Grams,P,T>`, then `Mass<T> = Mass<Grams,Kilo,T>`, then provisional `vMass = Mass<Grams,Kilo,double>`. These forms fix defaults rather than add semantics.

## Quantity-family interoperability

Cross-coordinate conversion is left-biased: a right Mass is converted into the left Unit, Prefix, and numeric carrier through `Unit.Scale`, `Prefix.Scale`, and `T.CreateChecked`. The result therefore remains in the left coordinate.

### C# 14 extension-operator pressure

The earlier conclusion that fully open operator syntax was blocked by C# was incomplete. On the current .NET 10 target, C# 14 extension blocks can introduce generic parameters inferred from the combined receiver and operator operands.

The probe uses a six-parameter extension block so both Mass operands contribute their Unit, Prefix, and numeric carrier. The semantic operation remains implemented by `Mass.Add` / `Mass.Subtract`; the extension block supplies only the C# operator realization.

Behavioral tests exercise `+` / `-` across same coordinates, different Prefix/carrier under one Unit, fully different Unit/Prefix/carrier within Mass, recommended `Mass<T>` descendants, and `vMass`.

The semantic policy remains checked and left-biased. Space Modeling CI run #23 verified the model, behavioral tests, and negative-compilation probe successfully.

The remaining question is semantic and ergonomic rather than syntactic: when does an operator hide too much conversion policy, even if C# can express and infer it?

## Current questions

The quantity-family experiment remains pressure, not a settled public API. Immediate questions include whether `Q<F,U,P,T>` adds genuine quantity-family semantics, which capabilities belong to Q versus concrete families, how extension-operator inference behaves for descendants and consumer types, whether static result types preserve enough nominal information, how Transformable participates in establishment, what role remains for the older `Quantity<DIM,PREFIX>` type, and later how dimensions compose through Product, Quotient, and Power.

## Verification

The normal probe builds with `dotnet build examples/VSlices.Space.Modeling/VSlices.Space.Modeling.csproj`; behavioral tests run through `tests/VSlices.Space.Modeling.Tests`; and the `MODELING_INVALID_USAGE` build is expected to fail. `.github/workflows/space-modeling.yml` exercises all three paths on pull requests.
