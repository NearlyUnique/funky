using Microsoft.CodeAnalysis;

namespace FunkyGen;

public partial class FunkyGenerator
{
    private readonly record struct FunkyContext(INamedTypeSymbol MockClass, INamedTypeSymbol TargetInterface)
    {
        public bool HasValue => MockClass is not null && TargetInterface is not null;
        public string MockClassName => MockClass.Name;
        public string TargetInterfaceName => TargetInterface.Name;
    }

    private sealed class FunkyContextEqualityComparer : IEqualityComparer<FunkyContext>
    {
        private FunkyContextEqualityComparer() { }

        public static FunkyContextEqualityComparer Instance { get; } = new();

        public bool Equals(FunkyContext x, FunkyContext y)
        {
            return x.HasValue && y.HasValue &&
                x.MockClass.Name == y.MockClass.Name &&
                   x.MockClass.ContainingNamespace.Name == y.MockClass.ContainingNamespace.Name &&
                   x.TargetInterface.Name == y.TargetInterface.Name &&
                   x.TargetInterface.ContainingNamespace.Name == y.TargetInterface.ContainingNamespace.Name;
        }

        public int GetHashCode(FunkyContext obj)
        {
            throw new NotImplementedException();
        }
    }
}
