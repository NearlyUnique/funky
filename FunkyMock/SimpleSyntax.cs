using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FunkyMock;

public static class SimpleSyntax
{
    public record class Arg(string Name, string Type)
    {
        public static readonly IList<Arg> None = new List<Arg>().AsReadOnly();
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
                    MethodKind.ReadProperty,
                    Array.Empty<Arg>());
            }
        }
    }
    /// <summary>
    /// From https://stackoverflow.com/a/27106959/14019
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string GetFullMetadataName(ISymbol? s)
    {
        if (s == null || IsRootNamespace(s))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!IsRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
            {
                sb.Insert(0, '+');
            }
            else
            {
                sb.Insert(0, '.');
            }

            sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            //sb.Insert(0, s.MetadataName);
            s = s.ContainingSymbol;
        }

        return sb.ToString();
    }

    private static bool IsRootNamespace(ISymbol symbol)
    {
        INamespaceSymbol? s = null;
        return (s = symbol as INamespaceSymbol) != null && s.IsGlobalNamespace;
    }
}
