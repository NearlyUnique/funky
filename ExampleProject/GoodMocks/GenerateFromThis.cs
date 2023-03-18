using System.Runtime.InteropServices.ComTypes;
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
        DateTime When { get; }
        string Name { get; set; }
        Task<int> SomeAsync();
        void AnAction(decimal dec);
    }
}

namespace ExampleProject.Tests
{
    using FunkyMock;
    using ExampleProject.GoodMocks;

    [Funky(typeof(IThing))]
    internal partial class AnyMocker
    {
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
                OnText = _ => "",
                OnAnAction = _ => { },
                OnSomeAsync = () => Task.FromResult(0),
                OnGetName = () => "the name",
                OnSetName = _ => { },
            };

            void ForceTestViaInterface(IThing thing)
            {
                Assert.IsTrue(thing.Predicate(0, new AnyType()));
                Assert.AreEqual("the name", mock.Name);
            }

            ForceTestViaInterface(mock);
        }
    }
}

