﻿using ExampleProject.Domain;

namespace ExampleProject.GoodMocks.HandCrafted;

/// <summary>
/// Simulated generated code
/// </summary>

internal partial class TheMock : IAnyInterface
{
    // Use Func<> & Action<>
    public Func<string, bool>? OnIsOk;
    public Func<int, SomeType, Task>? OnTaskAsync;
    public Func<int, decimal, Task<SomeType>>? OnTaskOfTAsync;

    public CallHistory Calls { get; } = new();

    public class CallHistory
    {
        public List<IsOkArgs> IsOk { get; } = new();
        public List<TaskASyncArgs> TaskASync { get; } = new();
        public List<TaskOfTAsyncArgs> TaskOfTAsync { get; } = new();

        public record IsOkArgs(string name);
        public record TaskASyncArgs(int value, SomeType some);
        public record TaskOfTAsyncArgs(int value1, decimal value2);
    }

    public bool IsOk(string name)
    {
        Calls.IsOk.Add(new CallHistory.IsOkArgs(name));
        if (OnIsOk == null)
        {
            return default;
        }
        return OnIsOk(name);
    }

    public async Task TaskAsync(int value, SomeType some)
    {
        Calls.TaskASync.Add(new CallHistory.TaskASyncArgs(value, some));
        if (OnTaskAsync == null)
        {
            await Task.CompletedTask;
            return;
        }
        await OnTaskAsync(value, some);
    }

    public Task<SomeType> TaskOfTAsync(int value1, decimal value2)
    {
        Calls.TaskOfTAsync.Add(new CallHistory.TaskOfTAsyncArgs(value1, value2));
        if (OnTaskOfTAsync is not null)
        {
            return OnTaskOfTAsync(value1, value2);
        }

        return Task.FromResult<SomeType>(default!);
    }
}
