# VSlices.Space.Modeling

This project is a modeling probe for the emerging `VSlices.Space` API. It is intentionally not an automated test project.

Its job is to make semantic decisions executable enough that their C# consequences can be inspected before those decisions become stable framework contracts.

## Current probe

`Location` exercises two concepts:

```text
Transformable<Location.Input, Location>
Evolvable<Location, Location.State>
```

The model intentionally separates:

```text
Input  = information required to establish a Location
State  = currently accepted state of an existing Location
```

Creation is target-owned:

```text
Location.Input -> Location
```

Evolution is persistent rather than mutating:

```text
Location(State0)
    + Func<State, State>
    -> candidate State
    -> evolution rules
    -> new Location(State1) | rejection
```

The original `Location` has no writable `State` and `Update` never receives authority to replace it. Accepted evolution materializes a new `Location`.

## State authority

`Location.State` has a private constructor. External code cannot mint a state from arbitrary data.

External code can still derive a candidate from a state it legitimately obtained:

```csharp
location.Update(state => state with { X = state.X + 1 });
```

This intentionally distinguishes:

```text
arbitrary data -> State              not allowed
accepted State -> candidate State    allowed
candidate State -> accepted Location controlled by Location.Evolution
```

## Negative compile probes

`InvalidUsage.cs` contains examples behind `MODELING_INVALID_USAGE` that are expected not to compile. They attempt to:

- call the private `Location.State` constructor;
- assign `Location.State` from outside the owner.

The normal modeling surface can be built with:

```text
dotnet build examples/VSlices.Space.Modeling/VSlices.Space.Modeling.csproj
```

To intentionally exercise the compiler barriers:

```text
dotnet build examples/VSlices.Space.Modeling/VSlices.Space.Modeling.csproj -p:DefineConstants=MODELING_INVALID_USAGE
```

The second command is expected to fail compilation. That failure is part of the probe: these restrictions should exist in the C# model itself rather than being asserted by runtime tests.
