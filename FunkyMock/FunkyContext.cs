using Microsoft.CodeAnalysis;

namespace FunkyMock;

public readonly record struct FunkyContext(INamedTypeSymbol MockClass, INamedTypeSymbol TargetInterface)
{
    public bool HasValue => MockClass is not null && TargetInterface is not null;
    public string MockClassName => MockClass.Name;
    public string TargetInterfaceName => TargetInterface.Name;
}
