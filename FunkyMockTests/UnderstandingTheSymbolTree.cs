using FunkyMock;
using FunkyMockTests.Helper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace FunkyMockTests;

public class UnderstandingTheSymbolTree
{
    [Fact]
    void extract_fully_qualified_namespace()
    {
        var source = @"
namespace ExampleProject.GoodMocks;
internal interface NotThisOne{}
internal interface IThing{}";

        var iface = new CodeFactory(source).Children<InterfaceDeclarationSyntax>(x => x.Identifier.Text == "IThing");

        string ns = SimpleSyntax.Namespace(iface);

        Assert.Equal("ExampleProject.GoodMocks", ns);
    }
    [Fact]
    void extract_class_accessibility_keyword()
    {
        var source = @"
namespace ExampleProject.GoodMocks;
internal class AnyInternal{}
protected internal class BothProtectedAndInternal{}
public class AnyPublic{}";

        var factory = new CodeFactory(source);

        var @public = factory.Children<ClassDeclarationSyntax>(x => x.Identifier.Text == "AnyPublic");
        Assert.NotNull(@public);
        Assert.Equal("public", SimpleSyntax.Accessibility(@public));

        var @internal = factory.Children<ClassDeclarationSyntax>(x => x.Identifier.Text == "AnyInternal");
        Assert.NotNull(@internal);
        Assert.Equal("internal", SimpleSyntax.Accessibility(@internal));

        var both = factory.Children<ClassDeclarationSyntax>(x => x.Identifier.Text == "BothProtectedAndInternal");
        Assert.NotNull(both);
        Assert.Equal("protected internal", SimpleSyntax.Accessibility(both));
    }
}
