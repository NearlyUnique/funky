# Overview

Mocking should be used sparingly but when you do use this technique it should be easy to do right and obvious how it works. If there is a problem with the test or the production code the Mocking system should help, not hinder.

# Mock types

There are Mocks, Stubs, Fakes and Test Doubles to name but a few. Here I use the word Mock to cover all types. The proposed approach does not limit you in any way to prefer other terminology or more precise meaning.

# Function based mocks

At it's core I prefer to mock any functions using replacement functions with very simple test specific behaviour ideally just returning simple values.

```c#
// the production interface
public interface IStockClient {
    StockItem? FindStockById(Guid id);
}

// some class under test
public class Controller {
    public Controller(IStockClient client) { }
    public Response HandleAddItem(Guid skuId) {
        // SNIP real logic
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
