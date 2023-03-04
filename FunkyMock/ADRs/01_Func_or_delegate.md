# ADR 01 Func or delegate

## Status : _Discussion_

## Context

The function pointers of the implemented mock could be as `Action`/`Func` or `delegate`. The main driver this question to ensure that declaring factory or helper functions is natural.

The objective is for Funky to be easy to use because it is simple straight forward normal C# code.

## Decision

Use `Func/Action`, despite having to differentiate (during generation) between void and non void methods, the consumer does not need to take particular heed either way. Factory functions still work without complex casting

The generated mock.
```csharp
internal partial class SimpleFuncMock : IAnyInterface
{
    public Func<string, bool>? OnIsOk;

    public bool IsOk(string name) => OnIsOk(name)
}
```
Example factory
```csharp
public partial class SimpleFuncMock {
    public static Func<string, bool> OkOnMonday(string dayOfWeek) =>
        _ => dayOfWeek == "Monday";
}
// usage
var mock = new SimpleFuncMock {
    OnIsOk = SimpleFuncMock.OkOnMonday("Monday"),
};
```
## Consequences

> _What becomes easier or more difficult to do because of this change?_
