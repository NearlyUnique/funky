using ExampleProject.GoodMocks;

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
        // properties not yet supported
        // DateTime When { get; }
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
        private int changeThisToForceRegeneration;
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
            var mock = new AnyMocker {
                OnPredicate = (_,_) => true,
            };

            void ForceTestViaInterface(IThing thing)
            {
                Assert.IsTrue(thing.Predicate(0, new AnyType()));
            }

            ForceTestViaInterface(mock);
        }
    }
}

