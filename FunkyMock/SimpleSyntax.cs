using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FunkyMock;

public static class SimpleSyntax
{
    public record Arg(string Name, string Type)
    {
        public override string ToString() => Type + " " + Name;
    }

    public record Method(string Name, string ReturnType, MethodKind Kind, IList<Arg> Args);

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

    /// <summary>
    /// Extract a simple representation of members for an interface
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static IEnumerable<Method> Members(INamedTypeSymbol symbol)
    {
        foreach (var member in symbol.GetMembers())
        {
            if (member is IMethodSymbol { MethodKind: Microsoft.CodeAnalysis.MethodKind.Ordinary } method)
            {
                yield return new Method(
                    method.Name,
                    method.ReturnType.ToDisplayString(),
                    MethodKind.Ordinary,
                    method.Parameters.Select(a => new Arg(a.Name, a.Type.ToDisplayString())).ToList().AsReadOnly());
            }
            else if (member is IPropertySymbol prop)
            {
                yield return new Method(
                    prop.Name,
                    prop.Type.ToDisplayString(),
                    SimplifyPropertyKind(prop),
                    Array.Empty<Arg>());
            }
        }
    }

    private static MethodKind SimplifyPropertyKind(IPropertySymbol prop)
    {
        var kind = MethodKind.ReadWrite;
        if (prop.IsReadOnly)
        {
            kind = MethodKind.ReadProperty;
        }

        if (prop.IsWriteOnly)
        {
            kind = MethodKind.WriteProperty;
        }

        return kind;
    }
}
