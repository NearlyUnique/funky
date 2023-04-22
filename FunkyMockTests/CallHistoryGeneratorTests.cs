using FunkyMock;
using FunkyMock.External;
using FunkyMockTests.Helper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace FunkyMockTests;

public class CallHistoryGeneratorTests
{
    [Fact]
    public void generate_method_field()
    {
        const string code = """
namespace Example {
    interface IContract {
        int Fn(string value);
    }
}
""";
        const string expected = """
public CallHistory Calls { get; } = new();

public class CallHistory
{
    public List<FnArgs> Fn { get; } = new();

    public record FnArgs(string value);
}

""";
        var methods = CodeFactory.Methods(code);
        var srcBuilder = new IndentedStringBuilder();
        CallHistoryGenerator.Generate(srcBuilder, methods);

        Assert.Equal(expected, srcBuilder.ToString());
    }
    [Fact]
    public void generate_property_field()
    {
        const string code = """
namespace Example {
    interface IContract {
        bool Property { get; set; }
    }
}
""";
        const string expected = """
public CallHistory Calls { get; } = new();

public class CallHistory
{
    public List<GetPropertyArgs> GetProperty { get; } = new();
    public List<SetPropertyArgs> SetProperty { get; } = new();

    public record GetPropertyArgs();
    public record SetPropertyArgs(bool value);
}

""";
        var methods = CodeFactory.Methods(code);
        var srcBuilder = new IndentedStringBuilder();
        CallHistoryGenerator.Generate(srcBuilder, methods);

        Assert.Equal(expected, srcBuilder.ToString());
    }
    [Fact]
    public void record_call_method()
    {
        const string code = """
namespace Example {
    interface IContract {
        void Fn(string text,int number);
    }
}
""";
        var methods = CodeFactory.Methods(code);

        Assert.Equal("Calls.Fn.Add(new CallHistory.FnArgs(text, number));", CallHistoryGenerator.GenerateAddCall(MethodKind.Ordinary, methods.Single(m => m.Name == "Fn")));
    }
    [Fact]
    public void record_call_property()
    {
        const string code = """
namespace Example {
    interface IContract {
        bool Property { get; set; }
    }
}
""";
        var methods = CodeFactory.Methods(code);

        var member = methods.Single(m => m.Name == "Property");

        Assert.Equal("Calls.GetProperty.Add(new CallHistory.GetPropertyArgs());", CallHistoryGenerator.GenerateAddCall(MethodKind.ReadProperty, member));
        Assert.Equal("Calls.SetProperty.Add(new CallHistory.SetPropertyArgs(value));", CallHistoryGenerator.GenerateAddCall(MethodKind.WriteProperty, member));
    }
}
