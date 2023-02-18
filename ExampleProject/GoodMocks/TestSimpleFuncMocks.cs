using ExampleProject.Domain;
using ExampleProject.GoodMocks.HandCrafted;
using NUnit.Framework;

namespace ExampleProject.GoodMocks;

public class TestingGoodMocks
{
    [Test]
    public async Task setup_call_and_assert()
    {
        string? capturedS = null;
        var mock = new SimpleFuncMock {
            OnIsOk = s => {
                capturedS = s;
                return true;
            },
            OnTaskOfTAsync = (_,_) => Task.FromResult(new SomeType{Name="ok"}),
            OnTaskAsync = (_,_) => throw new AnyErrorException(9),
        };

        var actual = await Domain.ProductionCode.ProductionFunction(mock);

        Assert.AreEqual("value", capturedS);
        Assert.AreEqual("ok", actual);

        // adjust behaviour
        mock.OnIsOk = _ => false;

        var ex = Assert.ThrowsAsync<AnyErrorException>(async () =>
            await Domain.ProductionCode.ProductionFunction(mock)
        );
        Assert.AreEqual("Any Error 9", ex?.Message);
        Assert.AreEqual(9, ex?.Count);
    }

    [Test]
    public void What_do_factory_functions_look_like()
    {
        bool LocalFunction(string s) => true;

        var mock = new SimpleFuncMock();

        mock.OnIsOk = LocalFunction;
        Assert.IsTrue(mock.IsOk("any"));

        mock.OnIsOk = MethodFunction;
        Assert.IsTrue(mock.IsOk("any"));

        mock.OnIsOk = FactoryFunction(10);
        Assert.IsTrue(mock.IsOk("any"));

        // inline lambda
        mock.OnIsOk = _ => {
            return true;
        };
        Assert.IsTrue(mock.IsOk("any"));
    }

    bool MethodFunction(string s) => true;

    private static Func<string, bool> FactoryFunction(int factoryParameter)
    {
        // expressed as a full function because factories are often more verbose (not complicated)
        // than a single expression
        return s => {
            return factoryParameter % 2 == 0;
        };
    }

    // Async things
    [Test]
    public async Task What_do_factory_functions_look_like_Async()
    {
        // Func<int, decimal,
        async Task<SomeType> LocalFunction(int i, decimal d) => await Task.FromResult(new SomeType { Name = "any local" });

        var mock = new SimpleFuncMock();

        mock.OnTaskOfTAsync = LocalFunction;
        Assert.AreEqual("any local", (await mock.TaskOfTAsync(1, 2m)).Name);

        mock.OnTaskOfTAsync = MethodFunctionAsync;
        Assert.AreEqual("any method", (await mock.TaskOfTAsync(1, 2m)).Name);

        mock.OnTaskOfTAsync = FactoryFunctionAsync("any async");
        Assert.AreEqual("any async", (await mock.TaskOfTAsync(1, 2m)).Name);

        // inline lambda
        mock.OnTaskOfTAsync = async (_,_) => await Task.FromResult(new SomeType { Name = "any inline" });
        Assert.AreEqual("any inline", (await mock.TaskOfTAsync(1, 2m)).Name);
    }

    async Task<SomeType> MethodFunctionAsync(int i, decimal d) => await Task.FromResult(new SomeType { Name = "any method" });

    private static Func<int, decimal, Task<SomeType>> FactoryFunctionAsync(string name)
    {
        // expressed as a full function because factories are often more verbose (not complicated)
        // than a single expression
        return async (int i, decimal d) => await Task.FromResult(new SomeType { Name = name });
    }
}
