using Microsoft.CodeAnalysis;

namespace FunkyMock;

internal sealed class ContextEqualityComparer : IEqualityComparer<FunkyContext>
{
    private ContextEqualityComparer() { }

    public static ContextEqualityComparer Instance { get; } = new();

    public bool Equals(FunkyContext x, FunkyContext y)
    {
        return x.HasValue && y.HasValue &&
               // The user can change their mock and not generate changes
               x.MockClass.Name == y.MockClass.Name &&
               x.MockClass.ContainingNamespace.Name == y.MockClass.ContainingNamespace.Name &&
               // any change to the interface forces a change
               SymbolEqualityComparer.Default.Equals(x.TargetInterface, y.TargetInterface);
    }

    public int GetHashCode(FunkyContext obj)
    {
        unchecked
        {
            var hash = 17;
            hash = hash * 31 + SymbolEqualityComparer.Default.GetHashCode(obj.MockClass);
            hash = hash * 31 + SymbolEqualityComparer.Default.GetHashCode(obj.TargetInterface);
            return hash;
        }
    }
}
