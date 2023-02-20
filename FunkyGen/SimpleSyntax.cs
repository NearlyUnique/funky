using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FunkyGen;

public static class SimpleSyntax
{
    /// <summary>
    /// C# keyword for the element accessibility (public, internal, etc.)
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string Accessibility(INamedTypeSymbol symbol) => SyntaxFacts.GetText(symbol.DeclaredAccessibility);

    /// <summary>
    /// The containing namespace or an empty string
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static string Namespace(INamedTypeSymbol? symbol) => symbol?.ContainingNamespace?.ToString() ?? "";
}
