using FunkyMock;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace FunkyMockTests.Helper;

internal sealed class CodeFactory
{
    private readonly SyntaxTree _tree;
    private readonly SemanticModel _semanticModel;

    public CodeFactory(string source)
    {
        _tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create("MyCompilation", new[] { _tree }, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
        _semanticModel = compilation.GetSemanticModel(_tree);
    }

    public INamedTypeSymbol? Children<T>(Func<T, bool>? filter = null) where T : TypeDeclarationSyntax =>
        _semanticModel.GetDeclaredSymbol(
            _tree.GetRoot().
                DescendantNodes().
                OfType<T>().
                First(filter ?? (_ => true)));

    public static IEnumerable<SimpleSyntax.Method> Methods(string source)
    {
        var children = new CodeFactory(source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(children);
        return SimpleSyntax.Members(children);
    }
}
