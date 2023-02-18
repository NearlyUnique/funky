namespace ExampleProject.Domain;

public static class ProductionCode
{
    public static async Task<string> ProductionFunction(IAnyInterface any)
    {
        if (any.IsOk("value"))
        {
            var x = await any.TaskOfTAsync(101, 202.202m);
            return x.Name??"default-value";
        }

        await any.TaskAsync(303, new SomeType { Name = "other-path" });

        return "not-executed";
    }
}

public class AnyErrorException : Exception
{
    public AnyErrorException(int c) :base($"Any Error {c}")=> Count = c;
    public int Count { get; }
}
