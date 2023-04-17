namespace FunkyMockTests.Experiments.Async;

using Xunit;

public class HandlersWithoutTask
{
    [Fact]
    public async Task without_task()
    {
        var fake = new FakeSomeAsyncDeTasked {
            OnFnInt = () => 10,
            OnFn = () => { },
        };

        Assert.Equal(10, await fake.FnInt() );
    }
    [Fact]
    public async Task with_task()
    {
        var fake = new FakeSomeAsync {
            OnFnInt = () => Task.FromResult(10),
            OnFn = () => Task.CompletedTask,
        };

        Assert.Equal(10, await fake.FnInt() );
    }

}

internal interface ISomeAsync
{
    Task Fn();
    Task<int> FnInt();
}
//[Funky]
// copied from generated code.
internal partial class FakeSomeAsync : ISomeAsync
{
    public Func<System.Threading.Tasks.Task>? OnFn;
    public Func<System.Threading.Tasks.Task<int>>? OnFnInt;

    public CallHistory Calls { get; } = new();

    public class CallHistory
    {
        public List<FnArgs> Fn { get; } = new();
        public List<FnIntArgs> FnInt { get; } = new();

        public record FnArgs();
        public record FnIntArgs();
    }

    public System.Threading.Tasks.Task Fn() {
        Calls.Fn.Add(new CallHistory.FnArgs());
        if (OnFn is null) { throw new System.NotImplementedException("'OnFn' has not been assigned"); }
        return OnFn();
    }
    public System.Threading.Tasks.Task<int> FnInt() {
        Calls.FnInt.Add(new CallHistory.FnIntArgs());
        if (OnFnInt is null) { throw new System.NotImplementedException("'OnFnInt' has not been assigned"); }
        return OnFnInt();
    }
}

// initial version copied from generated code.
internal partial class FakeSomeAsyncDeTasked : ISomeAsync
{
    //public Func<System.Threading.Tasks.Task>? OnFn;
    public Action? OnFn;
    //public Func<System.Threading.Tasks.Task<int>>? OnFnInt;
    public Func<int>? OnFnInt;

    public CallHistory Calls { get; } = new();

    public class CallHistory
    {
        public List<FnArgs> Fn { get; } = new();
        public List<FnIntArgs> FnInt { get; } = new();

        public record FnArgs();
        public record FnIntArgs();
    }

    public System.Threading.Tasks.Task Fn() {
        Calls.Fn.Add(new CallHistory.FnArgs());
        if (OnFn is null) { throw new System.NotImplementedException("'OnFn' has not been assigned"); }
        //return OnFn();
        OnFn();
        return Task.CompletedTask;
    }
    public System.Threading.Tasks.Task<int> FnInt() {
        Calls.FnInt.Add(new CallHistory.FnIntArgs());
        if (OnFnInt is null) { throw new System.NotImplementedException("'OnFnInt' has not been assigned"); }
        // return OnFnInt();
        return Task.FromResult<int>(OnFnInt());
    }
}
