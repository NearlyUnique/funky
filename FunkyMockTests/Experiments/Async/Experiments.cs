using Xunit;

namespace FunkyMockTests.Experiments.Async;

public class Experiments
{
    private System.Diagnostics.Stopwatch? _t;
    private const int ForceException = 42;
    [Fact]
    public void without_async()
    {
        _=ReadNumber(-1).Result;
        Reset();
        var x1 = ReadNumber(100);

        Log("pre test");
        Assert.Equal(100, x1.Result);
    }

    [Fact]
    public void without_async_exception()
    {
        _=ReadNumber(-1).Result;
        Reset();
        var x1 = ReadNumber(ForceException);

        Log("pre test");
        var ex = Assert.Throws<AggregateException>(() => _ = x1.Result);
        var ce = ex?.InnerExceptions.Single() as CustomException;
        Assert.NotNull(ce);
        Log($"caught: {ce?.Message}");
    }
    [Fact]
    public void with_async_exception()
    {
        _=ReadNumber(-1).Result;
        Reset();

        Log("pre test");
        var ce = Assert.ThrowsAsync<CustomException>(async () => await ReadNumber(ForceException));
        Assert.NotNull(ce);
        Log($"caught: {ce?.Result.Message}");
    }

    [Fact]
    public async Task with_async()
    {
        _=ReadNumber(-1).Result;
        Reset();
        var x1 = await ReadNumber(100);

        Log("pre test");
        Assert.Equal(100, x1);
    }

    [Fact]
    public void async_method_without_async()
    {
        _=ReadNumberAsync(-1).Result;
        Reset();
        var x1 = ReadNumberAsync(100);

        Log("pre test");
        Assert.Equal(100, x1.Result);
    }
    [Fact]
    public async Task async_method_with_async()
    {
        _=ReadNumberAsync(-1).Result;
        Reset();
        var x1 = await ReadNumberAsync(100);

        Log("pre test");
        Assert.Equal(100, x1);
    }
    [Fact]
    public void async_from_task_method_without_async()
    {
        _=ReadNumberAsyncFromResult(-1).Result;
        Reset();
        var x1 = ReadNumberAsyncFromResult(100);

        Log("pre test");
        Assert.Equal(100, x1.Result);
    }
    [Fact]
    public async Task async_from_task_method_with_async()
    {
        _=ReadNumberAsyncFromResult(-1).Result;
        Reset();
        var x1 = await ReadNumberAsyncFromResult(100);

        Log("pre test");
        Assert.Equal(100, x1);
    }

    Task<int> ReadNumber(int v)
    {
        try
        {
            Log($"> ReadNumber({v})");
            var task = Task<int>.Factory.StartNew(() => {
                try
                {
                    Log($"> ReadNumber({v})::Task");
                    if (v == 42)
                    {
                        var msg = $"throw ReadNumber({v})::Task";
                        Log(msg);
                        throw new CustomException(msg);
                    }

                    if (v > 0)
                    {
                        Task.Delay(v);
                    }

                    return v;
                }
                finally
                {
                    Log($"< ReadNumber({v})::Task");
                }
            });
            return task;
        }
        catch (Exception e)
        {
            Log($"ERROR ReadNumber({v}): {e}");
            throw;
        }
        finally
        {
            Log($"< ReadNumber({v})");
        }
    }
    async Task<int> ReadNumberAsync(int v)
    {
        try
        {
            Log($"> ReadNumberAsync({v})");
            return await Task<int>.Factory.StartNew(() => {
                try
                {
                    Log($"> ReadNumberAsync({v})::Task");
                    if (v == 42)
                    {
                        var msg = $"throw ReadNumberAsync({v})::Task";
                        Log(msg);
                        throw new CustomException(msg);
                    }
                    if (v > 0)
                    {
                        Task.Delay(v);
                    }

                    return v;
                }
                finally
                {
                    Log($"< ReadNumberAsync({v})::Task");
                }
            });
        }
        catch (Exception e)
        {
            Log($"ERROR ReadNumberAsync({v}): {e}");
            throw;
        }
        finally
        {
            Log($"< ReadNumberAsync({v})");
        }
    }

    async Task<int> ReadNumberAsyncFromResult(int v)
    {
        try
        {
            Log($"> ReadNumberAsyncFromResult({v})");
            return await Task.FromResult(v);
        }
        catch (Exception e)
        {
            Log($"ERROR ReadNumberAsyncFromResult({v}): {e}");
            throw;
        }
        finally
        {
            Log($"< ReadNumberAsyncFromResult({v})");
        }
    }
    Task<int> ReadNumberFromResult(int v)
    {
        try
        {
            Log($"> ReadNumberAsyncFromResult({v})");
            return Task.FromResult(v);
        }
        catch (Exception e)
        {
            Log($"ERROR ReadNumberAsyncFromResult({v}): {e}");
            throw;
        }
        finally
        {
            Log($"< ReadNumberAsyncFromResult({v})");
        }
    }

    private readonly object _lock= new();

    void Reset()
    {
        lock (_lock)
        {
            _t = null;
        }
    }

    private void Log(string s)
    {
        if (_t is null)
        {
            lock (_lock)
            {
                if (s.Contains('-'))
                {
                    return;
                }

                _t ??= System.Diagnostics.Stopwatch.StartNew();
            }
        }

        lock (_lock)
        {
            if (s.Contains('-'))
            {
                return;
            }
            Console.WriteLine(_t.Elapsed.TotalMicroseconds + " " + s);
        }
    }
}

internal class CustomException : Exception
{
    public CustomException(string message):base(message) {}
}
