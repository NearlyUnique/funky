# ADR 01 Func or delegate

## Status : _Discussion_

## Context

The function pointers of the implemented mock could be as `Action`/`Func` or `delegate`. The main driver for `delegate` is for declaring factory or helper functions.

The objective is for Funky to be easy to use because it is simple straight forward normal C# code.

## Decision

### Option `Func/Action`

The generated mock.
```csharp
internal partial class SimpleFuncMock : IAnyInterface
{
    public Func<string, bool>? OnIsOk;

    public bool IsOk(string name)
    {
        return OnIsOk?.Invoke(name) ?? default;
    }
}
```
A factory function
```csharp

```

### Option `delegate`

## Consequences

> _What becomes easier or more difficult to do because of this change?_
