using Xunit;

namespace FunkyMockTests.Experiments.Nested;

interface ISameNamespace
{
    int Fn(string s);
}

public partial class SomeTestClassWithNestedMock
{
    public partial class Mock : ISameNamespace { }

    [Fact]
    public void AnyTest()
    {
        var mock = new Mock { OnFn = _ => 10 };
        Assert.Equal(10, mock.Fn("any"));
    }
}

// What would the Generated Code look like
public partial class SomeTestClassWithNestedMock
{
    public partial class Mock : ISameNamespace
    {
        public Func<string, int>? OnFn;
        public int Fn(string s)
        {
            return OnFn!(s);
        }
    }
}
