# Overview

Testing with [mocks](#mock-types) should be used sparingly but when you do use this technique it should be easy to do right and obvious how it works. If there is a problem with the test or the production code the Mocking system should help, not hinder.

# Function based mocks

At it's core I prefer to mock any functions using replacement functions with very simple test specific behaviour ideally just returning simple values.

```c#
// the production interface
public interface IStockClient {
    StockItem? FindStockById(Guid id);
}

// some class under test
public class Controller {
    private readonly IStockClient _client;
    public Controller(IStockClient client) => _client = client;

    public Response HandleAddItem(Guid skuId) {
        if (!_client.FindStockById(skuId).InStock) {
            return new Response{ Error = "Out Of Stock" };
        }
        return new Response();
    }
}

// a test
[Test]
public void An_error_is_returned_for_out_of_stock_items() {
    var mock = new MockClient {
        // any stock item is "out of stock"
        OnFindStockById = _ => new StockItem { InStock = false }
    };
    var response = new Controller(mock).HandleAddItem(Guid.NewGuid());

    Assert.AreEqual("Out Of Stock", response.Error);
    // if you really want to see how many calls were made
    Assert.AreEqual(1, mock.Calls.FindStockById.Count);
}
```

You can see we implement as much as we need for mock. The mocked implementation is a simple function (or several functions) that is easy to write, read and debug. The interface parameters are captured and easy to assert on or ignore as you see fit.

All this is achieved with some Source Code Generation. All you need to do is define a `partial class` that implements the interface you care about and add the `[Funky]` attribute. Source generation creates the implementation and adds the source to your code for you to inspect, debug or ignore.

```c#
[Funky(typeof(IStockClient))]
internal partial class MockClient {}
```

You can use your part of the partial class to keep related static helper methods. e.g.

```c#
public partial class MockClient {
    public static StockItem? ReturnInStockItem(Guid id) =>
        new StockItem {
            Display = "Beans",
            InStock = true
        };
}

public void Any_test() {
    var mock = new MockClient {
        OnFindStockById = MockClient.ReturnInStockItem
    };
    // ...
}
```

# Mock types

There are Mocks, Stubs, Fakes and Test Doubles to name but a few. Here I use the word Mock to cover all types. The proposed approach does not limit you in any way to prefer other terminology or more precise meaning.

# Complex Mocks

This is my opinion on the current style of mocking frameworks. Historically I would have hand coded what I can now auto generate as the extra typing time saved complex mock test debugging time later.

This is a farly typical current approach with other mocking frameworks. If you pass the wrong uuid or don't specify `Any` or use the wrong type the test will just fail. The code that looks like a lambda isn't, it's an expression so is not designed to be executed, its designed to be examined by the framework. The frameworks encourage dense code blocks of setup where it is difficult to identify what is actually being tested. Simple DSL type factories can be built but are difficult to get right.

```csharp
var mock = new Mock<IStockClient>();
mock.Setup(x => x.FindStockById(It.IsAny<Guid>())).
    Returns(new StockItem()));

// run test

mock.Verify(x => x.FindStockById(specificUuid) );
```
