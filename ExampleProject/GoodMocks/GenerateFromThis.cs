namespace ExampleProject.GoodMocks
{

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
}

namespace ExampleProject.Tests
{
    using FunkyGen;
    using ExampleProject.GoodMocks;

    [Funky(typeof(IThing))]
    internal partial class AnyMocker
    {
        private int changeThisToForceRegeneration_;

        public static string Custom() => "custom code";
    }
}

namespace PlayArea
{
    using NUnit.Framework;
    using ExampleProject.Tests;

    public class Sandbox
    {
        [Test]
        public void Play()
        {
            var mock = new AnyMocker();

            Assert.AreEqual("generated code",AnyMocker.Generated());
            Assert.AreEqual("custom code",AnyMocker.Custom());
        }
    }
}
