using Nanover.Frontend.Input;
using NUnit.Framework;

namespace Nanover.Frontend.Tests.Input
{
    internal class DirectPosedObjectTests
    {
        [Test]
        public void PoseChanged_WithSingleChange_InvokedAfterSetPose()
        {
            var @object = new DirectPosedObject();
            var wasChanged = false;

            @object.PoseChanged += () => wasChanged = true;
            @object.SetPose(null);

            Assert.IsTrue(wasChanged);
        }
    }
}