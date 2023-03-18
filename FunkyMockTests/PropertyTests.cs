using FunkyMock;
using FunkyMockTests.Helper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace FunkyMockTests;

public class PropertyTests
{
    [Fact]
    void readonly_property()
    {
        var code = @"
namespace ExampleProject.Testing
{
    internal interface IThing
    {
        int Value {get;}
    }
}";
        var iface = new CodeFactory(code).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single();

        Assert.Equal("public int Value", SourceCode.Signature(m));
        Assert.Equal("if (OnGetValue is null) { throw new System.NotImplementedException(\"'OnGetValue' has not been assigned\"); }", SourceCode.ThrowIfNull(MethodKind.ReadProperty,m));

        Assert.Equal("public Func<int>? OnGetValue;", SourceCode.FuncPointer(MethodKind.ReadProperty, m));
        Assert.Equal("return OnGetValue();", SourceCode.InvokeFuncPointer(MethodKind.ReadProperty, m));
    }
    [Fact]
    void read_write_property()
    {
        var code = @"
namespace ExampleProject.Testing
{
    internal interface IThing
    {
        int Value {get;set;}
    }
}";
        var iface = new CodeFactory(code).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single();

        Assert.Equal("public int Value", SourceCode.Signature(m));
        Assert.Equal("if (OnSetValue is null) { throw new System.NotImplementedException(\"'OnSetValue' has not been assigned\"); }", SourceCode.ThrowIfNull(MethodKind.WriteProperty,m));

        Assert.Equal("public Action<int>? OnSetValue;", SourceCode.FuncPointer(MethodKind.WriteProperty, m));
        Assert.Equal("OnSetValue(value);", SourceCode.InvokeFuncPointer(MethodKind.WriteProperty, m));
    }
}
