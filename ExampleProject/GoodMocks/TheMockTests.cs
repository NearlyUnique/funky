using ExampleProject.Domain;
using ExampleProject.GoodMocks.HandCrafted;
using NUnit.Framework;

namespace ExampleProject.GoodMocks;

public class TheMockTests
{
    [Test]
    public async Task setup_call_and_assert()
    {
        var mock = new TheMock {
            OnIsOk = _ => true,
            OnTaskOfTAsync = (_,_) => Task.FromResult(new SomeType{Name="ok"}),
            OnTaskAsync = (_,_) => throw new AnyErrorException(9),
        };

        var actual = await Domain.ProductionCode.ProductionFunction(mock);

        Assert.AreEqual("value", mock.Calls.IsOk.First().name);
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
    public void readonly_property()
    {
        Assert.Throws<NotImplementedException>(() => { _ = new TheMock().Count; } );

        var mock = new TheMock {
            OnGetCount = () => 10,
        };
        Assert.AreEqual(10, mock.Count);
        Assert.AreEqual(1, mock.Calls.GetCount.Count);
    }
    [Test]
    public void what_would_automatic_read_write_implementation_look_like()
    {
        var rnd = new Random();
        var mock = new TheMock();

        Func<int>? defaultRead = () => {
            if (!mock.Calls.SetCount.Any())
                throw new InvalidOperationException(
                    "Default 'Get' implementation requires 'Count' has been set, alternatively implement 'OnGetCount' and 'OnSetCount'");
            return mock.Calls.SetCount.Last().value;
        };
        Action<int>? defaultWrite = _ => { };

        mock.OnGetCount = defaultRead;
        mock.OnSetCount = defaultWrite;

        Assert.Throws<InvalidOperationException>(() => _ = mock.Count);

        var value = rnd.Next();
        mock.Count = value;
        Assert.AreEqual(value, mock.Count);

        value = rnd.Next();
        mock.Count = value;
        Assert.AreEqual(value, mock.Count);
    }
}
