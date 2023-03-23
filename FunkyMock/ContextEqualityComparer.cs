namespace FunkyMock;

internal sealed class ContextEqualityComparer : IEqualityComparer<FunkyContext>
{
    private ContextEqualityComparer() { }

    public static ContextEqualityComparer Instance { get; } = new();

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
