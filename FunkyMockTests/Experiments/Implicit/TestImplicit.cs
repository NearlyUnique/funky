namespace FunkyMockTests.Experiments.Implicit;
using Xunit;

public class TestImplicit
{
    [Fact]
    public void use_implicit_mock()
    {
        var mock = new ImplicitMock {
            OnFn = _ => 9,
        };

        // generates compiler ERROR
        // error CS1061: 'ImplicitMock' does not contain a definition for 'Fn' and no accessible extension method 'Fn' accepting a first argument of type 'ImplicitMock' could be found (are you missing a using directive or an assembly reference?)
        //
        // mock.Fn("");

        ByInterface(mock);

        void ByInterface(IAnyInterface impl)
        {
            Assert.Equal(9, impl.Fn(""));
        }

    }
}

internal interface IAnyInterface
{
    int Fn(string text);
}

internal class ImplicitMock : IAnyInterface
{
    public Func<string, int>? OnFn;
    int IAnyInterface.Fn(string text)
    {
        return OnFn!(text);
    }
}
