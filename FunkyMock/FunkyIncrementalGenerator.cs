using System.Diagnostics;
using System.Runtime.CompilerServices;
using FunkyMock.Internal;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FunkyMock;

[Generator(LanguageNames.CSharp)]
public sealed class FunkyIncrementalGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(PostInitializationCallback);

        var config = context.AnalyzerConfigOptionsProvider.Select(ConfigPipeline.Select);

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(ProviderPredicate, ProviderTransform)
            .Combine(config)
            .Where(static v => v.Left.HasValue)
            .Select(SyntaxProviderTransformer)
            .WithComparer(ContextEqualityComparer.Instance);

        context.RegisterSourceOutput(provider, Execute);
    }

    /// <summary>
    /// Convert pipeline args to a FunkyContext
    /// </summary>
    /// <param name="selector">tuple containing the type info and config, type is a tuple of mockClass and targetInterface</param>
    /// <param name="_"></param>
    /// <returns></returns>
    private static FunkyContext SyntaxProviderTransformer(((INamedTypeSymbol mockClass, INamedTypeSymbol targetInterface)? types, Config config) selector, CancellationToken _)
    {
        if (selector.types?.mockClass is null || selector.types?.targetInterface is null)
        {
            return new FunkyContext(null!, null!, null!);
        }
        return new FunkyContext(selector.types?.mockClass, selector.types?.targetInterface, selector.config);
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
        Logger.Flush(context);
    }
}
