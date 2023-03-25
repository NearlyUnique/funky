﻿using ExampleProject.GoodMocks;

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
        string Name { get; set; }
        Task<int> SomeAsync();
        void AnAction(decimal dec);
    }
}

namespace ExampleProject.Tests
{
    using FunkyMock;
    using ExampleProject.GoodMocks;

    [Funky]
    internal partial class AnyMocker : IThing
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
                OnGetWhen = () => DateTime.Now,
                OnGetName = () => "the name",
                OnSetName = _ => { },
            };

            void ForceTestViaInterface(IThing thing)
            {
                Assert.IsTrue(thing.Predicate(0, new AnyType()));
                Assert.AreEqual("the name", mock.Name);
                Assert.AreEqual(1, mock.Calls.GetName.Count);
            }

            ForceTestViaInterface(mock);
        }
    }
}

