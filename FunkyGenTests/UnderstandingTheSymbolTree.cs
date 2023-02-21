using FunkyGen;
using FunkyGenTests.Helper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Xunit;

namespace FunkyGenTests;

public class UnderstandingTheSymbolTree
{
    [Fact]
    void extract_fully_qualified_namespace()
    {
        var source = @"
namespace ExampleProject.GoodMocks;
internal interface NotThisOne{}
internal interface IThing{}";

        var iface = new CodeFactory(source).Children<InterfaceDeclarationSyntax>(x => x.Identifier.Text == "IThing");

        string ns = SimpleSyntax.Namespace(iface);

        Assert.Equal("ExampleProject.GoodMocks", ns);
    }
    [Fact]
    void extract_class_accessibility_keyword()
    {
        var source = @"
namespace ExampleProject.GoodMocks;
internal class AnyInternal{}
protected internal class BothProtectedAndInternal{}
public class AnyPublic{}";

        var factory = new CodeFactory(source);

        var @public = factory.Children<ClassDeclarationSyntax>(x => x.Identifier.Text == "AnyPublic");
        Assert.NotNull(@public);
        Assert.Equal("public", SimpleSyntax.Accessibility(@public));

        var @internal = factory.Children<ClassDeclarationSyntax>(x => x.Identifier.Text == "AnyInternal");
        Assert.NotNull(@internal);
        Assert.Equal("internal", SimpleSyntax.Accessibility(@internal));

        var both = factory.Children<ClassDeclarationSyntax>(x => x.Identifier.Text == "BothProtectedAndInternal");
        Assert.NotNull(both);
        Assert.Equal("protected internal", SimpleSyntax.Accessibility(both));
    }

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
            Assert.Equal("void Void_Void()", m.Signature());
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

            Assert.Equal("int Int_String_Boolean(string text, bool truthy)", m.Signature());
        }
        [Fact]
        void with_generic_arguments()
        {
            var iface = new CodeFactory(_source).Children<InterfaceDeclarationSyntax>();
            Assert.NotNull(iface);

            var methods = SimpleSyntax.Members(iface);
            var m = methods.Single(x => x.Name == "Task_ofString_List_of_AClass");

            Assert.Equal("Task_ofString_List_of_AClass", m.Name);
            Assert.Equal("Task<string>", m.ReturnType);
            Assert.Equal(1, m.Args.Count);
            Assert.Equal("classy", m.Args[0].Name);
            Assert.Equal("List<AClass>", m.Args[0].Type);

            Assert.Equal("Task<string> Task_ofString_List_of_AClass(List<AClass> classy)", m.Signature());
        }

        [Fact]
        void generating_function_pointersfor_void_return()
        {
            var iface = new CodeFactory(_source).Children<InterfaceDeclarationSyntax>();
            Assert.NotNull(iface);

            var methods = SimpleSyntax.Members(iface);
            var m = methods.Single(x => x.Name == "Void_Void");

            Assert.Equal("Action? OnVoid_Void", m.FuncPointer());
        }
        [Fact]
        void generating_function_pointers_for_with_void_and_args()
        {
            var iface = new CodeFactory(_source).Children<InterfaceDeclarationSyntax>();
            Assert.NotNull(iface);

            var methods = SimpleSyntax.Members(iface);
            var m = methods.Single(x => x.Name == "Void_Int");

            Assert.Equal("Action<int>? OnVoid_Int", m.FuncPointer());
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

            Assert.Equal("Func<string, bool, int>? OnInt_String_Boolean", m.FuncPointer());
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

            Assert.Equal("OnVoid_Int(integer);", m.InvokeFuncPointer());
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

            Assert.Equal("return OnInt_String_Boolean(text, truthy);", m.InvokeFuncPointer());
        }
    }
}
