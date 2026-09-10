# VSlices.Space.Modeling

This project is a modeling probe for the emerging `VSlices.Space` API. It is intentionally not an automated test project.

Its job is to make semantic decisions executable enough that their C# consequences can be inspected before those decisions become stable framework contracts.

## Current probe

`Location` now composes three semantic concepts:

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

This is intentional pressure on the idea that `Input` may contain values from already-established semantic spaces rather than only primitives or external representations.

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
