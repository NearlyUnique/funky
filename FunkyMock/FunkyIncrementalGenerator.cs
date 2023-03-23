using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FunkyMock;

[Generator(LanguageNames.CSharp)]
public sealed partial class FunkyIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationCallback);

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(ProviderPredicate, ProviderTransform)
            .Where(static ((INamedTypeSymbol, INamedTypeSymbol)? types) => types.HasValue)
            .Select(static ((INamedTypeSymbol, INamedTypeSymbol)? types, CancellationToken _) => SyntaxProviderTransformer(types?.Item1, types?.Item2))
            .WithComparer(ContextEqualityComparer.Instance);

        context.RegisterSourceOutput(provider, Execute);
    }

    private static FunkyContext SyntaxProviderTransformer(INamedTypeSymbol? mockClass, INamedTypeSymbol? targetInterface)
    {
        if (mockClass is null || targetInterface is null)
        {
            return new FunkyContext(null!, null!);
        }
        return new FunkyContext(mockClass, targetInterface);
    }

    private static void PostInitializationCallback(IncrementalGeneratorPostInitializationContext context)
    {
        context.AddSource("FunkyAttribute.g.cs", StaticSource.FunkyMockAttributeSource);
    }

    private static bool ProviderPredicate(SyntaxNode syntaxNode, CancellationToken ct)
    {
        // Quick and dirty filter
        return syntaxNode is ClassDeclarationSyntax candidate
               && candidate.Modifiers.Any(SyntaxKind.PartialKeyword)
               && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
               && SimpleAttributeSyntax.HasAttribute(candidate, new[] { StaticSource.FunkyNamespaceShortName, StaticSource.FunkyNamespaceLongName });
    }

    private (INamedTypeSymbol, INamedTypeSymbol)? ProviderTransform(
        GeneratorSyntaxContext context,
        CancellationToken cancellationToken)
    {
        Debug.Assert(context.Node is ClassDeclarationSyntax);
        var candidate = Unsafe.As<ClassDeclarationSyntax>(context.Node);

        INamedTypeSymbol? symbol = context.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);

        if (symbol is not null
            && SimpleAttributeSyntax.TryGetAttribute(
                candidate,
                new[] { StaticSource.FunkyNamespaceShortName, StaticSource.FunkyNamespaceLongName },
                context.SemanticModel,
                cancellationToken)
            && symbol.Interfaces.Length == 1
           )
        {
            var type = symbol.Interfaces[0];
            return (symbol, type);
        }

        return null;
    }

    private static void Execute(SourceProductionContext context, FunkyContext funkyContext)
    {
        var code = SourceCode.Execute(funkyContext);
        context.AddSource($"{funkyContext.MockClassName}.g.cs", code);
    }
}
