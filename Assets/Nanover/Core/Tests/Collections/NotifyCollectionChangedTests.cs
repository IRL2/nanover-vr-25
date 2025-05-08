using System.Collections.Specialized;
using Nanover.Core.Collections;
using NUnit.Framework;

namespace Nanover.Core.Tests.Collections
{
    internal class NotifyCollectionChangedTests
    {
        private ObservableDictionary<string, object> dictionary
            = new ObservableDictionary<string, object>();

        private NotifyCollectionChangedEventArgs eventArgs;

        [SetUp]
        public void Setup()
        {
            dictionary = new ObservableDictionary<string, object>();
            dictionary.CollectionChanged += (sender, args) => eventArgs = args;
        }

        [Test]
        public void ClearEmptyDictionary()
        {
            dictionary.Clear();
            var (changed, removals) = eventArgs.AsChangesAndRemovals<string>();
            CollectionAssert.IsEmpty(changed);
            CollectionAssert.IsEmpty(removals);
        }

        [Test]
        public void AddItem()
        {
            dictionary.Add("abc", 1.4);
            var (changed, removals) = eventArgs.AsChangesAndRemovals<string>();
            CollectionAssert.AreEquivalent(new[]
            {
                "abc"
            }, changed);
            CollectionAssert.IsEmpty(removals);
        }

        [Test]
        public void RemoveItem()
        {
            dictionary.Add("abc", 1.4);
            dictionary.Remove("abc");
            var (changed, removals) = eventArgs.AsChangesAndRemovals<string>();
            CollectionAssert.IsEmpty(changed);
            CollectionAssert.AreEquivalent(new[]
            {
                "abc"
            }, removals);
        }
        
        [Test]
        public void SetItem()
        {
            dictionary.Add("abc", 1.4);
            eventArgs = null;
            dictionary["abc"] = "xyz";
            var (changed, removals) = eventArgs.AsChangesAndRemovals<string>();
            CollectionAssert.AreEquivalent(new[]
            {
                "abc"
            }, changed);
            CollectionAssert.IsEmpty(removals);
        }

        [Test]
        public void Clear()
        {
            dictionary.Add("abc", 1.4);
            dictionary.Add("def", "xyz");
            eventArgs = null;
            dictionary.Clear();
            var (changed, removals) = eventArgs.AsChangesAndRemovals<string>();
            CollectionAssert.IsEmpty(changed);
            CollectionAssert.AreEquivalent(new[]
            {
                "abc", "def"
            }, removals);
        }
    }
}