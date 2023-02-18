using ExampleProject.Domain;

namespace ExampleProject.GoodMocks.HandCrafted;

internal partial class TheMock
{
    public static async Task<SomeType> RedGreenBlue(int i, decimal d)
    {
        return await Task.FromResult(new SomeType { Name = "RGB" });
    }

    public static Func<int, decimal, Task<SomeType>>? OrangeBanana(string name)
    {
        return (Func<int, decimal, Task<SomeType>>)((i, d) =>
            Task.FromResult(new SomeType { Name = "OB" }));
    }

    public static Func<int, decimal, Task<SomeType>> OrangeBanana3(string name) =>
        (i, d) => Task.FromResult(new SomeType { Name = "OB" });

    public static AnyTypeAsyncFunc OrangeBanana2(string name) =>
        (i, d) => Task.FromResult(new SomeType { Name = "OB2" });
}
