# VSlices.Space.Modeling

This project is a modeling probe for the emerging `VSlices.Space` API. It is intentionally not an automated test project.

Its job is to make semantic decisions executable enough that their C# consequences can be inspected before those decisions become stable framework contracts.

## Current probes

The project currently contains two kinds of pressure:

1. `Location`, which explores semantic establishment, evolution, and authority.
2. Quantitative modeling, which now contains both the original `Quantity<DIM, PREFIX>` experiment and a parallel `Q<F, U, P, T>` quantity-family experiment.

The second experiment deliberately exists beside the first one rather than replacing it immediately. The point is to compare what each geometry makes easy or awkward before promoting either shape into framework API.

## Location probe

`Location` composes three semantic concepts:

```text
string -> LocationName
Location.Input -> Location
Location(State0) -> Location(State1)
```

through:

```text
LocationName : DiscreteSpace<LocationName>, Transformable<string, LocationName>
Location     : Transformable<Location.Input, Location>, Evolvable<Location, Location.State>
```

`LocationName` is an independently established semantic value. Its own rules are:

```text
non-empty
maximum length: 100 characters
```

The transformation trims surrounding whitespace before materializing the accepted name.

This means `Location` no longer accepts a primitive `string` as its name input and does not duplicate name invariants. Its input requires an already-established `LocationName`:

```text
string
  -> LocationName
  -> Location.Input(LocationName, ...)
  -> Location
```

## Input and State

The model intentionally separates:

```text
Input = information required to establish a Location
State = currently accepted state of an existing Location
```

`State` does not mean that every member is evolvable. A state may contain both information fixed when the first point is established and information callers are allowed to propose changes to later.

The current `Location.State` makes that distinction explicit:

```text
Name = creation-fixed / never replaceable after establishment
X/Y  = evolvable through Update
```

Accordingly, `Name` is get-only, while `X` and `Y` remain `init` properties so `with` can produce candidate states for those parts.

Evolution is persistent rather than mutating:

```text
Location(State0)
    + Func<State, State>
    -> candidate State
    -> evolution rules
    -> new Location(State1) | rejection
```

The original `Location` has no writable `CurrentState`; accepted evolution materializes a new `Location`.

## State authority

`Location.State` has a private constructor. External code cannot mint a state from arbitrary data.

C# does not grant an enclosing type privileged access to private members of its nested type, so `Location` cannot directly call `new Location.State(...)` either. The probe intentionally keeps the constructor private and bridges this realization limitation through .NET's `UnsafeAccessor` support:

```csharp
[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
private static extern State NewState(LocationName name, int x, int y);
```

This accessor is private to `Location`; it preserves the public restriction while allowing the owning semantic type to materialize a state internally.

External code can derive candidates only through members that `State` deliberately exposes as evolvable:

```csharp
location.Update(state => state with
{
    X = state.X + 1,
    Y = state.Y + 1
});
```

but cannot propose a different creation-fixed name:

```csharp
state with { Name = anotherName } // expected compile failure
```

This intentionally distinguishes:

```text
arbitrary external data -> State           not allowed
Location-owned materialization -> State    allowed through private runtime accessor
accepted State -> candidate X/Y            allowed
accepted State -> replacement Name         not allowed
candidate State -> accepted Location       controlled by Location.Evolution
```

The use of `UnsafeAccessor` is treated as a .NET realization mechanism, not as part of the semantic model. If a simpler language-level mechanism later preserves the same authority boundary, the realization may change without changing the semantics.

## Name authority

`LocationName` also has a private constructor, but unlike `State` its target-owned `Transformable<string, LocationName>` rules live inside `LocationName` itself. Therefore it can directly call its own constructor after its invariants succeed; no `UnsafeAccessor` is needed.

This gives a useful contrast:

```text
string -> LocationName  owned and materialized by LocationName itself
Input  -> Location      owned by Location
State  -> Location'     owned by Location, with State construction bridged by .NET realization
```

## Quantity-family probe

A new parallel experiment lives under:

```text
QuantityFamilies/QuantityFamily.cs
```

Its current hypothesis is that the useful abstraction is not necessarily an instantiable `Quantity<DIM, PREFIX>` value. Instead, a quantity can expose structural family membership through:

```csharp
Q<F, U, P, T>
```

with:

```text
F = dimensional family
U = unit
P = prefix
T = backing numeric type
```

The current contract deliberately stays small:

```csharp
public interface Q<F, U, P, T>
    where F : Dimension
    where U : Unit<F>
    where P : Prefix
    where T : INumberBase<T>
```

`Q` only states structural membership and the numeric carrier requirement. Concrete quantitative algebra may require stronger constraints.

### Dimension as a type hierarchy

The probe represents dimensions as a type hierarchy rather than marker structs:

```text
Dimension
└── Dimension.Mass
```

This keeps the dimension available to generic constraints without introducing a separate runtime descriptor/witness distinction.

### Unit, Prefix, and backing type

Units are constrained to their dimension:

```csharp
Unit<Dimension.Mass>
```

so an incompatible unit cannot participate in a mass family merely because it has a scale.

The probe currently contains:

```text
Units:   Grams, Pounds
Prefixes: None, Kilo, Micro
Carriers: any T satisfying the relevant .NET generic-math contracts
```

`Q` asks only for `INumberBase<T>`, because family membership needs a numeric carrier but not necessarily the full ordered arithmetic surface.

`Mass<U, P, T>` currently strengthens that to `INumber<T>` because its implementation actually performs arithmetic.

### Conceptual family ordering

The generic order is intentionally:

```text
Mass<U, P, T>
```

rather than ordering arguments by defaulting convenience.

The current reason is conceptual proximity:

```text
Mass<Grams, None, decimal>
```

is treated as structurally closer to:

```text
Mass<Grams, Micro, double>
```

than to:

```text
Mass<Pounds, None, decimal>
```

because Unit establishes a stronger quantity-family neighborhood than Prefix or backing carrier.

### Recommended specializations

The current inheritance probe expresses progressively stronger VSlices recommendations:

```text
abstract Mass<U, P, T>
    consumer chooses Unit, Prefix, and carrier

abstract Mass<P, T>
    = Mass<Grams, P, T>
    VSlices fixes the recommended Unit

Mass<T>
    = Mass<Grams, Kilo, T>
    VSlices fixes Unit + Prefix

vMass
    = Mass<Grams, Kilo, double>
    VSlices fixes Unit + Prefix + carrier
```

`vMass` remains a deliberately provisional name while `LanguageExt.Mass` occupies the simple `Mass` name in overlapping contexts.

These recommended forms add defaults rather than new semantics.

### Arithmetic pressure

The probe intentionally distinguishes cheap interoperability from increasingly distant interoperability.

Exact coordinate addition is an operator on the most general family:

```text
Mass<U, P, T> + Mass<U, P, T>
    -> Mass<U, P, T>
```

Cross-coordinate addition is currently explicit:

```csharp
left.Add(right)
```

and supports a right operand with different:

```text
Unit
Prefix
backing numeric type
```

provided both Units belong to `Dimension.Mass` and numeric conversion into the left carrier succeeds through .NET generic math.

The current policy is left-biased:

```text
Mass<U1, P1, T1>.Add(Mass<U2, P2, T2>)
    -> Mass<U1, P1, T1>
```

The right coordinate is converted into the left Unit/Prefix/carrier before arithmetic.

This is deliberately not yet exposed as a universal cross-shape `+` operator. C# operators cannot introduce their own generic parameters, so an open operation of the form:

```text
Mass<U1, P1, T1> + Mass<U2, P2, T2>
```

cannot be expressed by a single generic operator declaration in the same way that a generic method can. That target-language constraint is evidence for the design rather than a reason to hide conversion policy.

The useful question is therefore becoming:

```text
which interoperability deserves operator syntax,
and which should remain an explicit family conversion/addition operation?
```

The probe currently tests:

- family membership through `Q<Dimension.Mass, ...>`;
- exact Unit/Prefix/carrier addition through the generic Mass base;
- same-Unit addition across Prefix and numeric carrier;
- fully open addition across Unit, Prefix, and numeric carrier;
- left-biased result coordinates.

## Negative compile probes

`InvalidUsage.cs` contains examples behind `MODELING_INVALID_USAGE` that are expected not to compile. They attempt to:

- call the private `Location.State` constructor;
- call the private `LocationName` constructor instead of using its transformation;
- change creation-fixed `State.Name` through `with`;
- replace `Location.CurrentState` from outside the owner.

The normal modeling surface can be built with:

```text
dotnet build examples/VSlices.Space.Modeling/VSlices.Space.Modeling.csproj
```

To intentionally exercise the compiler barriers:

```text
dotnet build examples/VSlices.Space.Modeling/VSlices.Space.Modeling.csproj -p:DefineConstants=MODELING_INVALID_USAGE
```

The second command is expected to fail compilation. That failure is part of the probe: these restrictions should exist in the C# model itself rather than being asserted by runtime tests.
