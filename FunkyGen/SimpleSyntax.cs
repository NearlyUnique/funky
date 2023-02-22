using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FunkyGen;

public static class SimpleSyntax
{
    public record class Arg(string Name, string Type)
    {
        public static readonly IList<Arg> None = new List<Arg>().AsReadOnly();
        public override string ToString() => Type + " " + Name;
    }

    public record class Method(string Name, string ReturnType, IList<Arg> Args)
    {
        /// <summary>
        /// public {returnType} {name}(arg list)
        /// </summary>
        /// <returns></returns>
        public string Signature() => $"public {ReturnType} {Name}({string.Join(", ", Args)})";

        /// <summary>
        /// public [Func|Action] &lt;{type list}&gt;;
        /// </summary>
        /// <returns></returns>
        public string FuncPointer()
        {
            var funcType = "Action";
            var args = new List<string>();

            args.AddRange(Args.Select(x => x.Type));
            if (ReturnType != "void")
            {
                funcType = "Func";
                args.Add(ReturnType);
            }
            string typeParams = "";
            if (args.Any())
            {
                typeParams = "<"+string.Join(", ", args)+">";
            }

            return $"public {@funcType}{typeParams}? On{Name};";
        }

        /// <summary>
        /// (return) On{name}(args);
        /// </summary>
        /// <returns></returns>
        public string InvokeFuncPointer()
        {
            var @return = "return ";
            if (ReturnType == "void")
            {
                @return = "";
            }

            return $"{@return}On{Name}({string.Join(", ", Args.Select(x => x.Name))});";
        }

        public string ThrowIfNull() => $"if (On{Name} is null) {{ throw new System.NotImplementedException(\"'On{Name}' has not been assigned\"); }}";
    }

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
            if (member is IMethodSymbol method)
            {
                yield return new Method(
                    method.Name,
                    method.ReturnType.ToDisplayString(),
                    method.Parameters.Select(a => {
                        return new Arg(a.Name, a.Type.ToDisplayString());
                    }).ToList().AsReadOnly());
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
