using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FunkyMock;

internal static class SimpleAttributeSyntax
{
    internal static bool TryGetAttribute(
        ClassDeclarationSyntax candidate,
        IList<string> attributeNames,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        foreach (var attributeList in candidate.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                var t = semanticModel.GetTypeInfo(attribute, cancellationToken);
                if (t.Type != null && attributeNames.Contains(t.Type.Name))
                {
                    return true;
                }
            }
        }

        return false;
    }

    internal static bool HasAttribute(ClassDeclarationSyntax candidate, string[] attributeNames)
    {
        foreach (var attributeList in candidate.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (attribute.Name is IdentifierNameSyntax ins && attributeNames.Contains(ins.Identifier.ValueText))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
