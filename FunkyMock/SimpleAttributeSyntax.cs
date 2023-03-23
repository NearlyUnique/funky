using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FunkyMock;

internal class SimpleAttributeSyntax
{
    internal static bool TryGetAttribute(
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
                TypeInfo t = semanticModel.GetTypeInfo(attribute, cancellationToken);
                if (t.Type != null && attributeNames.Contains(t.Type.Name))
                {
                    value = attribute;
                    return true;
                }
            }
        }

        value = null;
        return false;
    }

    internal static bool HasAttribute(ClassDeclarationSyntax candidate, string[] attributeNames)
    {
        foreach (AttributeListSyntax attributeList in candidate.AttributeLists)
        {
            foreach (AttributeSyntax attribute in attributeList.Attributes)
            {
                if (attribute.Name is IdentifierNameSyntax ins)
                {
                    if (attributeNames.Contains(ins.Identifier.ValueText))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
