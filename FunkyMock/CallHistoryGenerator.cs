using FunkyMock.External;

namespace FunkyMock;

public static class CallHistoryGenerator
{
    public static void Generate(IndentedStringBuilder srcBuilder, IEnumerable<SimpleSyntax.Method> methods)
    {
        srcBuilder
            .AppendLine("public CallHistory Calls { get; } = new();")
            .AppendLine()
            .AppendLine("public class CallHistory")
            .AppendLine("{")
            .IncrementIndent();

        foreach (var method in methods)
        {
            DefineList(srcBuilder, method);
        }

        srcBuilder.AppendLine();

        foreach (var method in methods)
        {
            DefineRecord(srcBuilder, method);
        }

        srcBuilder
            .DecrementIndent()
            .AppendLine("}");
    }

    private static void DefineRecord(IndentedStringBuilder srcBuilder, SimpleSyntax.Method method)
    {
        WriteDefinition(method, (name, args) => srcBuilder.AppendLine($"public record {name}Args({string.Join(", ", args)});"));
    }

    private static void DefineList(IndentedStringBuilder srcBuilder, SimpleSyntax.Method method)
    {
        WriteDefinition(method, (name, _) => srcBuilder.AppendLine($"public List<{name}Args> {name} {{ get; }} = new();"));
    }

    private static void WriteDefinition(SimpleSyntax.Method method, Action<string, IList<SimpleSyntax.Arg>> applyTemplate)
    {
        if (method.Kind == MethodKind.Ordinary)
        {
            applyTemplate(method.Name, method.Args);
        }
        if ((method.Kind & MethodKind.ReadProperty) != 0)
        {
            applyTemplate("Get" + method.Name, Array.Empty<SimpleSyntax.Arg>());
        }
        if ((method.Kind & MethodKind.WriteProperty) != 0)
        {
            applyTemplate("Set" + method.Name, new SimpleSyntax.Arg[]{ new ("value", method.ReturnType)});
        }
    }

    public static string GenerateAddCall(MethodKind kind, SimpleSyntax.Method method) =>
        (method.Kind & kind) switch {
            MethodKind.Ordinary => $"Calls.{method.Name}.Add(new CallHistory.{method.Name}Args({string.Join(", ", method.Args.Select(x => x.Name))}));",
            MethodKind.ReadProperty => $"Calls.Get{method.Name}.Add(new CallHistory.Get{method.Name}Args());",
            MethodKind.WriteProperty => $"Calls.Set{method.Name}.Add(new CallHistory.Set{method.Name}Args(value));",
            _ => throw new InvalidOperationException($"'{kind} not set on method {method.Kind}"),
        };
}
