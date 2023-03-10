using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
}
