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
        public string Signature() => $"{ReturnType} {Name}({string.Join(", ", Args)})";

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

            return $"{@funcType}{typeParams}? On{Name}";
        }

        public string InvokeFuncPointer()
        {
            var @return = "return ";
            if (ReturnType == "void")
            {
                @return = "";
            }

            return $"{@return}On{Name}({string.Join(", ", Args.Select(x => x.Name))});";
        }
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

}
