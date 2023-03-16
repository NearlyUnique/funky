namespace ExampleProject.Domain;

public interface IAnyInterface
{
    public bool IsOk(string name);
    public Task TaskAsync(int value, SomeType some);
    public Task<SomeType> TaskOfTAsync(int value1, decimal value2);
    public int Count { get; }
}
