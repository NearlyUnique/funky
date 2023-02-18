using ExampleProject.Domain;
using NUnit.Framework;

namespace ExampleProject.GoodMocks;

public class MockingTest
{
    [Test]
    public async Task HandCraftedInterfaceSignatures()
    {
        var mock = new GoodMocks.HandCrafted.TheMock {
            OnIsOk = _ => true,
            OnTaskAsync = async (i, st) => await Task.CompletedTask,
            OnTaskOfTAsync2 = async (i, d) => await Task.FromResult(new SomeType { Name = $"x({i},{d})" }),
        };
        Assert.AreEqual(0, mock.Calls.IsOk.Count);
        Assert.True(mock.IsOk("from main"));
        Assert.AreEqual(1, mock.Calls.IsOk.Count);
        Assert.AreEqual("from main", mock.Calls.IsOk[0].name);

        Assert.AreEqual(0, mock.Calls.TaskASync.Count);
        await mock.TaskAsync(13, new SomeType{Name = "a-name"});

        Assert.AreEqual(1, mock.Calls.TaskASync.Count);
        Assert.AreEqual("a-name", mock.Calls.TaskASync[0].some.Name);

        Assert.AreEqual(0, mock.Calls.TaskOfTAsync.Count);
        var actual1 = await mock.TaskOfTAsync(199, 12.1m);
        var actual2 = await mock.TaskOfTAsync(200, 12.2m);
        Assert.AreEqual(2, mock.Calls.TaskOfTAsync.Count);

        Assert.AreEqual(199, mock.Calls.TaskOfTAsync[0].value1);
        Assert.AreEqual(12.1m, mock.Calls.TaskOfTAsync[0].value2);
        Assert.AreEqual("x(199,12.1)", actual1.Name);

        Assert.AreEqual(200, mock.Calls.TaskOfTAsync[1].value1);
        Assert.AreEqual(12.2m, mock.Calls.TaskOfTAsync[1].value2);
        Assert.AreEqual("x(200,12.2)", actual2.Name);
    }

    [Test]
    public async Task Test_RedGreenBlue()
    {
        var mock = new GoodMocks.HandCrafted.TheMock{
            OnTaskOfTAsync2 = GoodMocks.HandCrafted.TheMock.RedGreenBlue,
        };

        await mock.TaskOfTAsync(0, 0m);
    }
    [Test]
    public async Task Test_OrangeBanana()
    {
        var mock = new GoodMocks.HandCrafted.TheMock {
            OnTaskOfTAsync = GoodMocks.HandCrafted.TheMock.OrangeBanana("one"),
        };

        await mock.TaskOfTAsync(0, 0m);
    }
    [Test]
    public async Task Test_OrangeBanana2()
    {
        var mock = new GoodMocks.HandCrafted.TheMock {
            OnTaskOfTAsync2 = GoodMocks.HandCrafted.TheMock.OrangeBanana2("two"),
        };

        await mock.TaskOfTAsync(0, 0m);
    }

}
public static class ProductionCode
{
    public static async Task<string> ProductionFunction(IAnyInterface any)
    {
        if (any.IsOk("value"))
        {
            var x = await any.TaskOfTAsync(42, 1.23m);
            return x.Name??"";
        }

        return "";
    }
}


// // Moq
// var mockStockChecker = new Mock<IStockChecker>();
// mockStockChecker.Setup(x => x.IsProductInStock()).Returns(true);
// var sut = new TheClassIAmTesting(mockStockChecker.Object);
// sut.DoSomething();
//
// // NSubstitute
// var mockStockChecker = Substitute.For<IStockChecker>();
// mockStockChecker.IsProductInStock().Returns(true);
// var sut = new TheClassIAmTesting(mockStockChecker);
// sut.DoSomething();
//
// // FakeItEasy
// var mockStockChecker = A.Fake<IStockChecker>();
// A.CallTo(() => mockStockChecker.IsProductInStock("")).Returns(true);
// var sut = new TheClassIAmTesting(mockStockChecker);
// sut.DoSomething();

// Better
