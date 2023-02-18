namespace ExampleProject.GoodMocks;

internal class AnyType
{
    public string? Word { get; set; }
}

internal interface IThing
{
    string Text(int number);
    bool Predicate(float f, AnyType anyType);
    DateTime When { get; }
    Task<int> SomeAsync();
    void AnAction(decimal dec);
}

// [FunkyGen.Funky(typeof(IThing))]
internal partial class AnyMocker { }
