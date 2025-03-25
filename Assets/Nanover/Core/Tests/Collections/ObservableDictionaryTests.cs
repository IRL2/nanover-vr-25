using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Nanover.Core.Collections;
using Nanover.Testing;
using NSubstitute;
using NUnit.Framework;

namespace Nanover.Core.Tests.Collections
{
    internal class ObservableDictionaryTests
    {
        [Test]
        public void DefaultConstructor_IsEmptyDictionary()
        {
            var dictionary = new ObservableDictionary<string, int>();
            Assert.AreEqual(0, dictionary.Count);
        }

        private void HasHandlerReceived(NotifyCollectionChangedEventHandler handler,
                                        NotifyCollectionChangedAction action,
                                        string newItem = null,
                                        string oldItem = null)
        {
            Predicate<NotifyCollectionChangedEventArgs> predicate =
                args =>
                    args.Action == action
                 && (newItem == null || (string) args.NewItems[0] == newItem)
                 && (oldItem == null || (string) args.OldItems[0] == oldItem);
            ;

            handler.Received().Invoke(Arg.Any<object>(),
                                      Arg.Is<NotifyCollectionChangedEventArgs>(
                                          args => predicate(args)));
        }

        [Test]
        public void Event_SetIndexer()
        {
            var handler = Substitute.For<NotifyCollectionChangedEventHandler>();
            var dictionary = new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            dictionary.CollectionChanged += handler;

            dictionary["c"] = 0;

            HasHandlerReceived(handler, NotifyCollectionChangedAction.Add, newItem: "c");
        }

        [Test]
        public void Event_Add()
        {
            var handler = Substitute.For<NotifyCollectionChangedEventHandler>();
            var dictionary = new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            dictionary.CollectionChanged += handler;

            dictionary.Add("c", 1);

            HasHandlerReceived(handler, NotifyCollectionChangedAction.Add, newItem: "c");
        }

        [Test]
        public void Event_Add_KeyValuePair()
        {
            var handler = Substitute.For<NotifyCollectionChangedEventHandler>();
            var dictionary = new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            dictionary.CollectionChanged += handler;

            dictionary.Add(new KeyValuePair<string, int>("c", 1));

            HasHandlerReceived(handler, NotifyCollectionChangedAction.Add, newItem: "c");
        }

        [Test]
        public void Event_Replace()
        {
            var handler = Substitute.For<NotifyCollectionChangedEventHandler>();
            var dictionary = new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            dictionary.CollectionChanged += handler;

            dictionary["a"] = 1;

            HasHandlerReceived(handler,
                               NotifyCollectionChangedAction.Replace,
                               oldItem: "a",
                               newItem: "a");
        }

        [Test]
        public void Event_Remove()
        {
            var handler = Substitute.For<NotifyCollectionChangedEventHandler>();
            var dictionary = new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            dictionary.CollectionChanged += handler;

            dictionary.Remove("a");

            HasHandlerReceived(handler, NotifyCollectionChangedAction.Remove, oldItem: "a");
        }

        [Test]
        public void Event_Remove_KeyValuePair()
        {
            var handler = Substitute.For<NotifyCollectionChangedEventHandler>();
            var dictionary = new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            dictionary.CollectionChanged += handler;

            dictionary.Remove(new KeyValuePair<string, int>("a", 0));

            HasHandlerReceived(handler, NotifyCollectionChangedAction.Remove, oldItem: "a");
        }

        [Test]
        public void Event_Clear()
        {
            var handler = Substitute.For<NotifyCollectionChangedEventHandler>();
            var dictionary = new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            dictionary.CollectionChanged += handler;

            dictionary.Clear();

            HasHandlerReceived(handler, NotifyCollectionChangedAction.Remove);
        }

        [TestFixture]
        public class NewDictionaryTests : DictionaryImplementationTests
        {
            protected override IDictionary<string, int> Setup()
            {
                return new ObservableDictionary<string, int> { ["a"] = 0, ["b"] = 1 };
            }
        }

        [TestFixture]
        public class ExistingDictionaryTests : DictionaryImplementationTests
        {
            protected override IDictionary<string, int> Setup()
            {
                var existingDictionary = new Dictionary<string, int> { ["a"] = 0, ["b"] = 1 };
                return new ObservableDictionary<string, int>(existingDictionary);
            }
        }
    }
}