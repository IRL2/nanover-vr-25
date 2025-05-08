using System.Collections.ObjectModel;
using Nanover.Visualisation.Utility;
using NUnit.Framework;

namespace Nanover.Visualisation.Tests
{
    internal class CollectionDirtyStateTests
    {
        private ObservableCollection<int> collection;
        private CollectionDirtyState<int> dirtyState;

        [SetUp]
        public void Setup()
        {
            collection = new ObservableCollection<int> { 0, 1, 2 };
            dirtyState = new CollectionDirtyState<int>(collection);
        }

        [Test]
        public void InitialDirtyItems()
        {
            CollectionAssert.AreEquivalent(collection, dirtyState.DirtyItems);
        }

        [Test]
        public void ClearDirty()
        {
            dirtyState.ClearAllDirty();

            CollectionAssert.AreEquivalent(new int[0], dirtyState.DirtyItems);
        }

        [Test]
        public void Add_DirtyItems()
        {
            collection.Add(3);

            CollectionAssert.AreEquivalent(new[] { 0, 1, 2, 3 }, dirtyState.DirtyItems);
        }

        [Test]
        public void Add_DirtyItems_New()
        {
            dirtyState.ClearAllDirty();

            collection.Add(3);

            CollectionAssert.AreEquivalent(new[] { 3 }, dirtyState.DirtyItems);
        }

        [Test]
        public void Remove_DirtyItems()
        {
            collection.Remove(1);

            CollectionAssert.AreEquivalent(new[] { 0, 2 }, dirtyState.DirtyItems);
        }

        [Test]
        public void Replace_DirtyItems()
        {
            collection[0] = 3;

            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, dirtyState.DirtyItems);
        }

        [Test]
        public void IsDirty()
        {
            Assert.IsTrue(dirtyState.IsDirty(0));
            Assert.IsFalse(dirtyState.IsDirty(3));
        }

        [Test]
        public void ClearDirtyState()
        {
            dirtyState.ClearDirty(1);

            Assert.IsTrue(dirtyState.IsDirty(0));
            Assert.IsFalse(dirtyState.IsDirty(1));
        }

        [Test]
        public void MarkDirty()
        {
            dirtyState.ClearAllDirty();

            dirtyState.MarkDirty(1);

            Assert.IsFalse(dirtyState.IsDirty(0));
            Assert.IsTrue(dirtyState.IsDirty(1));
        }

        [Test]
        public void SetDirty()
        {
            Assert.IsTrue(dirtyState.IsDirty(0));

            dirtyState.SetDirty(0, false);

            Assert.IsFalse(dirtyState.IsDirty(0));

            dirtyState.SetDirty(0, true);

            Assert.IsTrue(dirtyState.IsDirty(0));
        }

        [Test]
        public void IsDirtyKey()
        {
            dirtyState.SetDirty(0, false);

            Assert.IsFalse(dirtyState[0]);
            Assert.IsTrue(dirtyState[1]);
        }
    }
}