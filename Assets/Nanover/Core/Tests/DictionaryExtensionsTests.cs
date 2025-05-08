using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Nanover.Core.Tests
{
    internal class DictionaryExtensionsTests
    {
        [Test]
        public void Deconstruct()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", 0 }
            };

            foreach (var (key, value) in dict)
            {
                Assert.AreEqual("a", key);
                Assert.AreEqual(0, value);
            }
        }

        [Test]
        public void GetValueOrDefault_ReferenceTypeAvailable()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", "test" }
            };

            Assert.AreEqual("test", dict.GetValueOrDefault<string>("a"));
        }

        [Test]
        public void GetValueOrDefault_ReferenceTypeMissing()
        {
            var dict = new Dictionary<string, object>();

            Assert.IsNull(dict.GetValueOrDefault<string>("a"));
        }

        [Test]
        public void GetValueOrDefault_ReferenceTypeWrong()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", new[] { 1f } }
            };

            Assert.Throws<InvalidOperationException>(() => dict.GetValueOrDefault<string>("a"));
        }

        [Test]
        public void GetValueOrDefault_ReferenceTypeCastable()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", "test" }
            };

            Assert.IsNotNull(dict.GetValueOrDefault<IEnumerable<char>>("a"));
        }

        [Test]
        public void GetValueOrDefault_ValueTypeAvailable()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", 1 }
            };

            Assert.AreEqual(1, dict.GetValueOrDefault<int>("a"));
        }

        [Test]
        public void GetValueOrDefault_ValueTypeMissing()
        {
            var dict = new Dictionary<string, object>();

            Assert.AreEqual(default(int), dict.GetValueOrDefault<int>("a"));
        }

        [Test]
        public void GetValueOrDefault_ValueTypeWrong()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", "1" }
            };

            Assert.Throws<InvalidOperationException>(() => dict.GetValueOrDefault<int>("a"));
        }

        [Test]
        public void GetValueOrDefault_ValueTypeCastable()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", 1 }
            };

            Assert.AreEqual(1f, dict.GetValueOrDefault<int>("a"));
        }

        [Test]
        public void GetArrayOrEmpty_Available()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", new[] { 0, 2, 3 } }
            };

            CollectionAssert.AreEqual(new[] { 0, 2, 3 }, dict.GetArrayOrEmpty<int>("a"));
        }

        [Test]
        public void GetArrayOrEmpty_Missing()
        {
            var dict = new Dictionary<string, object>();

            CollectionAssert.AreEqual(new int[0], dict.GetArrayOrEmpty<int>("a"));
        }

        [Test]
        public void GetValueOrDefault_Wrong()
        {
            var dict = new Dictionary<string, object>()
            {
                { "a", new[] { 1f, 2f, 3f } }
            };

            Assert.Throws<InvalidOperationException>(() => dict.GetArrayOrEmpty<int>("a"));
        }
    }
}