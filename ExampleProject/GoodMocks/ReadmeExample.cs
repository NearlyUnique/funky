using NUnit.Framework;

namespace ExampleProject.GoodMocks;

public class ReadmeExample
{
    public class StockItem
    {
        public bool InStock;
        public string Display;
    }

    public class Response
    {
        public string Error;
    };

    // temp until it all works
    public partial class MockClient : IStockClient
    {
        public Func<Guid, StockItem?> OnFindStockById;
        public StockItem? FindStockById(Guid id) => null;
        public CallHistory Calls { get; } = new();

        public class CallHistory
        {
            public List<FindStockByIdArgs> FindStockById { get; } = new();
            public record FindStockByIdArgs(Guid id);
        }
    }

//------ START Readme.me -----------------

    // the production interface
    public interface IStockClient
    {
        StockItem? FindStockById(Guid id);
    }

    // some class under test
    public class Controller
    {
        public Controller(IStockClient client) { }
        public Response HandleAddItem(Guid skuId)
        {
            // real logic
            return new Response();
        }
    }

    // a test
    [Test]
    public void An_error_is_returned_for_out_of_stock_items()
    {
        var mock = new MockClient {
            // any stock item is "out of stock"
            OnFindStockById = _ => new StockItem { InStock = false }
        };
        var response = new Controller(mock).HandleAddItem(Guid.NewGuid());

        Assert.AreEqual("Out Of Stock", response.Error);
        // if you really want to see how many calls were made
        Assert.AreEqual(1, mock.Calls.FindStockById.Count);
    }
    // -------------- part 2 -----------------
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
    //------ END Readme.me -----------------
}
