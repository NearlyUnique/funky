using FunkyMock;
using FunkyMockTests.Helper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace FunkyMockTests;

public class ListMethods
{
    const string _source = @"
namespace ExampleProject.GoodMocks
{
    class AClass { }
}
namespace ExampleProject.Testing
{
    internal interface IThing
    {
        void Void_Void();
        void Void_Int(int integer);
        int Int_String_Boolean(string text, bool truthy);
        Task<string> Task_ofString_List_of_AClass(List<AClass> classy);
    }
}";

    [Fact]
    void with_no_arguments()
    {
        var iface = new CodeFactory(_source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Void_Void");

        Assert.Equal("Void_Void", m.Name);
        Assert.Equal("void", m.ReturnType);
        Assert.Empty(m.Args);
        Assert.Equal("public void Void_Void()", SourceCode.Signature(m));
    }
    [Fact]
    void with_basic_arguments()
    {
        var iface = new CodeFactory(_source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Int_String_Boolean");

        Assert.Equal("Int_String_Boolean", m.Name);
        Assert.Equal("int", m.ReturnType);
        Assert.Equal(2, m.Args.Count);
        Assert.Equal("text", m.Args[0].Name);
        Assert.Equal("string", m.Args[0].Type);
        Assert.Equal("truthy", m.Args[1].Name);
        Assert.Equal("bool", m.Args[1].Type);

        Assert.Equal("public int Int_String_Boolean(string text, bool truthy)", SourceCode.Signature(m));
    }
    [Fact]
    void with_generic_arguments()
    {
        const string source = @"using
                                    namespace ExampleProject.GoodMocks {
                                        class AClass { }
                                    }
                                    namespace ExampleProject.Testing {
                                        internal interface IThing {
                                            Task<string> Task_ofString_List_of_AClass(List<AClass> classy);
                                        }
                                    }
";
        var iface = new CodeFactory(source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Task_ofString_List_of_AClass");

        Assert.Equal("Task_ofString_List_of_AClass", m.Name);
        Assert.Equal("Task<string>", m.ReturnType);
        Assert.Equal(1, m.Args.Count);
        Assert.Equal("classy", m.Args[0].Name);
        Assert.Equal("List<AClass>", m.Args[0].Type);

        Assert.Equal("public Task<string> Task_ofString_List_of_AClass(List<AClass> classy)", SourceCode.Signature(m));
    }
    [Fact]
    void generating_function_pointers_for_void_return()
    {
        var iface = new CodeFactory(_source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Void_Void");

        Assert.Equal("public Action? OnVoid_Void;", SourceCode.FuncPointer(MethodKind.Ordinary, m));
    }
    [Fact]
    void generating_function_pointers_for_with_void_and_args()
    {
        var iface = new CodeFactory(_source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Void_Int");

        Assert.Equal("public Action<int>? OnVoid_Int;", SourceCode.FuncPointer(MethodKind.Ordinary, m));
    }
    [Fact]
    void generating_function_pointers_for_with_typed_return()
    {
        const string source = @"namespace ExampleProject.GoodMocks;
                                    internal interface IThing{
                                        int Int_String_Boolean(string text, bool truthy);
                                    }";
        var iface = new CodeFactory(source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Int_String_Boolean");

        Assert.Equal("public Func<string, bool, int>? OnInt_String_Boolean;", SourceCode.FuncPointer(MethodKind.Ordinary, m));
    }
    [Fact]
    void invoke_the_function_pointer_for_void()
    {
        const string source = @"namespace ExampleProject.GoodMocks;
                                    internal interface IThing {
                                        void Void_Int(int integer);
                                    }
                                    ";
        var iface = new CodeFactory(source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Void_Int");

        Assert.Equal("OnVoid_Int(integer);", SourceCode.InvokeFuncPointer(m));
    }
    [Fact]
    void invoke_the_function_pointer_for_return_type()
    {
        const string source = @"
namespace ExampleProject.GoodMocks;
internal interface IThing {
    int Int_String_Boolean(string text, bool truthy);
}
";
        var iface = new CodeFactory(source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Int_String_Boolean");

        Assert.Equal("return OnInt_String_Boolean(text, truthy);", SourceCode.InvokeFuncPointer(m));
    }
    [Fact]
    void throw_if_func_is_unassigned()
    {
        var iface = new CodeFactory(_source).Children<InterfaceDeclarationSyntax>();
        Assert.NotNull(iface);

        var methods = SimpleSyntax.Members(iface);
        var m = methods.Single(x => x.Name == "Void_Void");

        Assert.Equal("if (OnVoid_Void is null) { throw new System.NotImplementedException(\"'OnVoid_Void' has not been assigned\"); }",
            SourceCode.ThrowIfNull(MethodKind.Ordinary, m));
    }
}
