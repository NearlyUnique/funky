using ExampleProject.Domain;

namespace ExampleProject.GoodMocks.HandCrafted;

internal class SimpleDelegateMock : IAnyInterface
{
    public delegate bool IsOkFunc(string name);
    public delegate Task TaskAsyncFunc(int i, SomeType some);
    public delegate Task<SomeType> TaskOfTAsyncFunc(int value1, decimal value2);

    public IsOkFunc? _IsOk;
    public TaskAsyncFunc? _TaskAsync;
    public TaskOfTAsyncFunc? _TaskOfTAsync;


    public bool IsOk(string name)
    {
        return _IsOk?.Invoke(name) ?? default;
    }

    public Task TaskAsync(int value, SomeType some)
    {
        return _TaskAsync?.Invoke(value, some) ?? Task.CompletedTask;
    }

    public Task<SomeType> TaskOfTAsync(int value1, decimal value2)
    {
        if (_TaskOfTAsync is null)
        {
            Task.FromResult<SomeType>(null!);
        }

        return _TaskOfTAsync?.Invoke(value1, value2)!;
    }
}
