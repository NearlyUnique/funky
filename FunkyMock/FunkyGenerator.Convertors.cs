using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FunkyMock;

public partial class FunkyGenerator
{
    private static bool TryGetAttribute(
        ClassDeclarationSyntax candidate,
        IList<string> attributeNames,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        [NotNullWhen(true)] out AttributeSyntax? value)
    {
        foreach (AttributeListSyntax attributeList in candidate.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                // SymbolInfo info = semanticModel.GetSymbolInfo(attribute, cancellationToken);
                TypeInfo t = semanticModel.GetTypeInfo(attribute, cancellationToken);
                if (t.Type != null && attributeNames.Contains(t.Type.Name))
                {
                    value = attribute;
                    return true;
                }
                // ISymbol? symbol = info.Symbol;
                //
                // if (symbol is IMethodSymbol method
                //     && method.ContainingType.ToDisplayString().Equals(attributeName, StringComparison.Ordinal))
                // {
                //     value = attribute;
                //     return true;
                // }
            }
        }

        value = null;
        return false;
    }

    private static bool TryGetType(
        AttributeSyntax attribute,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        [NotNullWhen(true)] out INamedTypeSymbol? value)
    {
        if (attribute.ArgumentList is
            {
                Arguments.Count: 1,
            } argumentList)
        {
            AttributeArgumentSyntax argument = argumentList.Arguments[0];

            if (argument.Expression is TypeOfExpressionSyntax typeOf)
            {
                SymbolInfo info = semanticModel.GetSymbolInfo(typeOf.Type, cancellationToken);
                ISymbol? symbol = info.Symbol;

                if (symbol is INamedTypeSymbol type)
                {
                    value = type;
                    return true;
                }
            }
        }

        value = null;
        return false;
    }
}
