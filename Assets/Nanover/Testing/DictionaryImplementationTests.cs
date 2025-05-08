using System.Collections.Generic;
using NUnit.Framework;

namespace Nanover.Testing
{
    /// <summary>
    /// General tests of an IDictionary implementation. Override Setup() to provide the
    /// dictionary
    /// </summary>
    [Ignore("")]
    public abstract class DictionaryImplementationTests
    {
        protected IDictionary<string, int> Dictionary;

        [SetUp]
        public void TestSetup()
        {
            Dictionary = Setup();
        }

        protected abstract IDictionary<string, int> Setup();

        [Test]
        public void GetIndexer()
        {
            Assert.AreEqual(0, Dictionary["a"]);
            Assert.AreEqual(1, Dictionary["b"]);
        }

        [Test]
        public void SetIndexer()
        {
            Dictionary["c"] = 2;
            Assert.AreEqual(3, Dictionary.Count);
            Assert.AreEqual(2, Dictionary["c"]);
        }

        [Test]
        public void Keys()
        {
            Assert.AreEqual(new[] { "a", "b" }, Dictionary.Keys);
        }

        [Test]
        public void Values()
        {
            Assert.AreEqual(new[] { 0, 1 }, Dictionary.Values);
        }

        [Test]
        public void Add()
        {
            Dictionary.Add("c", 2);
            Assert.AreEqual(3, Dictionary.Count);
            Assert.AreEqual(2, Dictionary["c"]);
        }

        [Test]
        public void ContainsKey()
        {
            Assert.IsTrue(Dictionary.ContainsKey("b"));
            Assert.IsFalse(Dictionary.ContainsKey("c"));
        }

        [Test]
        public void Remove()
        {
            Dictionary.Remove("b");
            Assert.AreEqual(1, Dictionary.Count);
            Assert.IsFalse(Dictionary.ContainsKey("b"));
        }

        [Test]
        public void TryGetValue()
        {
            var hasA = Dictionary.TryGetValue("a", out var valueA);
            Assert.IsTrue(hasA);
            Assert.AreEqual(0, valueA);

            var hasC = Dictionary.TryGetValue("c", out _);
            Assert.IsFalse(hasC);
        }

        [Test]
        public void Count()
        {
            Assert.AreEqual(2, Dictionary.Count);
        }
    }
}