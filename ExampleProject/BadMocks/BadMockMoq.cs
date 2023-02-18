using ExampleProject.Domain;
using Moq;
using NUnit.Framework;

namespace ExampleProject.BadMocks;

public class BadMockMoq
{
    [Test]
    public void BasicUsage()
    {
        var mocked = new Mock<IAnyInterface>();
        mocked.Setup(x => x.IsOk(It.IsAny<string>())).Returns(true);
        mocked.Setup(x => x.TaskAsync(10, It.IsAny<SomeType>())).Returns(Task.CompletedTask);
    }

    [Test]
    public async Task setup_call_and_assert()
    {
        var mock = new Mock<IAnyInterface>();
        mock.Setup(x => x.IsOk("value")).Returns(true);
        mock.Setup(x => x.TaskOfTAsync(It.IsAny<int>(), It.IsAny<decimal>())).
            Returns(Task.FromResult(new SomeType{Name="ok"}));
        mock.Setup(x => x.TaskAsync(It.IsAny<int>(), It.IsAny<SomeType>())).
            Throws(new AnyErrorException(9));

        var actual = await Domain.ProductionCode.ProductionFunction(mock.Object);
        Assert.AreEqual("ok", actual);

        // Adjust behaviour
        mock.Setup(x => x.IsOk(It.IsAny<string>())).Returns(false);

        var ex = Assert.ThrowsAsync<AnyErrorException>(async () =>
            await Domain.ProductionCode.ProductionFunction(mock.Object));

        Assert.AreEqual("Any Error 9", ex?.Message);
        Assert.AreEqual(9, ex?.Count);
        mock.Verify(x => x.IsOk("value") );
    }
}
