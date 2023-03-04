﻿using FunkyMock.External;

namespace FunkyMock;

/// <summary>
/// Methods to write C# source code
/// </summary>
public static class SourceCode
{
    /// <summary>
    /// I'm not happy about adding this. But currently I cannot work out how to fully qualify all types found on the interface
    /// </summary>
    public const string GlobalUsing = """
// Standard global using
using global::System;
using global::System.Collections.Generic;
using global::System.IO;
using global::System.Linq;
using global::System.Net.Http;
using global::System.Threading;
using global::System.Threading.Tasks;
""";

    /// <summary>
    /// public {returnType} {name}(arg list)
    /// </summary>
    /// <returns></returns>
    public static string Signature(SimpleSyntax.Method m) => $"public {m.ReturnType} {m.Name}({string.Join(", ", m.Args)})";

    /// <summary>
    /// public [Func|Action] &lt;{type list}&gt;;
    /// </summary>
    /// <returns></returns>
    public static string FuncPointer(SimpleSyntax.Method m)
    {
        var funcType = "Action";
        var args = new List<string>();

        args.AddRange(m.Args.Select(x => x.Type));
        if (m.ReturnType != "void")
        {
            funcType = "Func";
            args.Add(m.ReturnType);
        }
        string typeParams = "";
        if (args.Any())
        {
            typeParams = "<"+string.Join(", ", args)+">";
        }

        return $"public {@funcType}{typeParams}? On{m.Name};";
    }

    /// <summary>
    /// (return) On{name}(args);
    /// </summary>
    /// <returns></returns>
    public static string InvokeFuncPointer(SimpleSyntax.Method m)
    {
        var @return = "return ";
        if (m.ReturnType == "void")
        {
            @return = "";
        }

        return $"{@return}On{m.Name}({string.Join(", ", m.Args.Select(x => x.Name))});";
    }

    /// <summary>
    /// if ( On{MethodName} is null ) throw Exception();
    /// </summary>
    /// <param name="m"></param>
    /// <returns></returns>
    public static string ThrowIfNull(SimpleSyntax.Method m) => $"if (On{m.Name} is null) {{ throw new System.NotImplementedException(\"'On{m.Name}' has not been assigned\"); }}";

    public static string Execute(FunkyContext source)
    {
        var targetInterface = source.TargetInterface;

        var implementation = new IndentedStringBuilder();
        var pointers = new IndentedStringBuilder();

        pointers.IncrementIndent();
        implementation.IncrementIndent();

        foreach (var member in SimpleSyntax.Members(targetInterface))
        {
            pointers.AppendLine(FuncPointer(member));

            implementation.Append(Signature(member))
                .AppendLine(" {")
                .IncrementIndent()
                .AppendLine(ThrowIfNull(member))
                .AppendLine(InvokeFuncPointer(member))
                .DecrementIndent()
                .AppendLine("}");
        }


        var text = @$"// <auto-generated/>
#nullable enable
namespace {SimpleSyntax.Namespace(source.MockClass)};

{GlobalUsing}

{SimpleSyntax.Accessibility(source.MockClass)} partial class {source.MockClassName} : {SimpleSyntax.Namespace(source.TargetInterface)}.{source.TargetInterfaceName}
{{
{pointers}
{implementation}
}}";
        return text;
    }

}
