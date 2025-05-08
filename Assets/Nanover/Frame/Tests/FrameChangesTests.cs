using Nanover.Frame.Event;
using NUnit.Framework;

namespace Nanover.Frame.Tests
{
    internal class FrameChangesTests
    {
        [Test]
        public void Initial_HasAnythingChanged()
        {
            var changes = FrameChanges.None;
            Assert.IsFalse(changes.HasAnythingChanged);
        }

        [Test]
        public void Initial_HasRandomChanged()
        {
            var changes = FrameChanges.None;
            Assert.IsFalse(changes.HasChanged("id"));
        }

        [Test]
        public void SetIsChanged()
        {
            var changes = FrameChanges.None;
            changes.MarkAsChanged("id");
            Assert.IsTrue(changes.HasChanged("id"));
        }

        [Test]
        public void SetIsChanged_HasAnythingChanged()
        {
            var changes = FrameChanges.None;
            changes.MarkAsChanged("id");
            Assert.IsTrue(changes.HasAnythingChanged);
        }

        [Test]
        public void Merge_ChangesWithEmpty()
        {
            var original = FrameChanges.None;
            var next = FrameChanges.None;
            next.MarkAsChanged("id");
            original.MergeChanges(next);
            Assert.IsTrue(original.HasChanged("id"));
        }
        
        [Test]
        public void MergeAllIntoNone()
        {
            var original = FrameChanges.None;
            var next = FrameChanges.All;
            original.MergeChanges(next);
            Assert.IsTrue(original.HasChanged("id"));
        }

        [Test]
        public void Merge_EmptyWithChanges()
        {
            var original = FrameChanges.None;
            original.MarkAsChanged("id");

            var next = FrameChanges.None;

            original.MergeChanges(next);
            Assert.IsTrue(original.HasChanged("id"));
        }
    }
}