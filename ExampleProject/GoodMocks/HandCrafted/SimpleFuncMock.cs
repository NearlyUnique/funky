using ExampleProject.Domain;

namespace ExampleProject.GoodMocks.HandCrafted;

internal class SimpleFuncMock : IAnyInterface
{
    public Func<string, bool>? OnIsOk;
    public Func<int, SomeType, Task>? OnTaskAsync;
    public Func<int, decimal, Task<SomeType>>? OnTaskOfTAsync;

    public bool IsOk(string name)
    {
        return OnIsOk?.Invoke(name) ?? default;
    }

    public Task TaskAsync(int value, SomeType some)
    {
        return OnTaskAsync?.Invoke(value, some) ?? Task.CompletedTask;
    }

    public Task<SomeType> TaskOfTAsync(int value1, decimal value2)
    {
        if (OnTaskOfTAsync is null)
        {
            Task.FromResult<SomeType>(null!);
        }

        return OnTaskOfTAsync?.Invoke(value1, value2)!;
    }
}
