using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Math;
using Nanover.Frontend.Manipulation;
using Nanover.Testing;
using NUnit.Framework;
using UnityEngine;

namespace Nanover.Frontend.Tests
{
    internal class ManipulableTransformTests
    {
        private static IEnumerable<bool> GrabReleaseSequence =>
            RandomTestData.SeededRandom(() => Random.value > 0.5f).Take(64);

        [Test]
        public void StartGrabManipulation_WithNoGrabs_AllowsGrab()
        {
            var transform = new GameObject("Test Transform").transform;
            var manipulable = new ManipulableTransform(transform);

            Assert.IsNotNull(manipulable.StartGrabManipulation(UnitScaleTransformation.identity));
        }

        [Test]
        public void StartGrabManipulation_WithOneGrab_AllowsGrab()
        {
            var transform = new GameObject("Test Transform").transform;
            var manipulable = new ManipulableTransform(transform);

            var pose1 = UnitScaleTransformation.identity;
            var pose2 = UnitScaleTransformation.identity;
            pose2.position += Vector3.right;

            manipulable.StartGrabManipulation(pose1);

            Assert.IsNotNull(manipulable.StartGrabManipulation(pose2));
        }

        [Test]
        public void StartGrabManipulation_WithTwoGrabs_RejectsGrab()
        {
            var transform = new GameObject("Test Transform").transform;
            var manipulable = new ManipulableTransform(transform);

            var pose1 = UnitScaleTransformation.identity;
            var pose2 = UnitScaleTransformation.identity;
            var pose3 = UnitScaleTransformation.identity;
            pose2.position += Vector3.right;
            pose3.position += Vector3.up;

            manipulable.StartGrabManipulation(pose1);
            manipulable.StartGrabManipulation(pose2);

            Assert.IsNull(manipulable.StartGrabManipulation(pose3));
        }

        [Test]
        public void StartGrabManipulation_WithReleasedGrabs_AllowsGrab()
        {
            var transform = new GameObject("Test Transform").transform;
            var manipulable = new ManipulableTransform(transform);

            var pose1 = UnitScaleTransformation.identity;
            var pose2 = UnitScaleTransformation.identity;
            var pose3 = UnitScaleTransformation.identity;
            pose2.position += Vector3.right;
            pose3.position += Vector3.up;

            manipulable.StartGrabManipulation(pose1);
            var grab = manipulable.StartGrabManipulation(pose2);
            grab.EndManipulation();

            Assert.IsNotNull(manipulable.StartGrabManipulation(pose3));
        }

        [Test]
        public void
            StartGrabManipulation_WithRandomGrabReleaseSequence_NeverAllowsMoreThanTwoGrabs()
        {
            var transform = new GameObject("Test Transform").transform;
            var manipulable = new ManipulableTransform(transform);
            var grabs = new List<IActiveManipulation>();

            foreach (var grabMore in GrabReleaseSequence)
            {
                if (grabMore)
                {
                    var pose = SpatialTestData.GetRandomTransformation();
                    var grab = manipulable.StartGrabManipulation(pose.AsUnitTransformWithoutScale());

                    if (grab != null)
                    {
                        grabs.Add(grab);
                    }
                }
                else if (grabs.Count > 0)
                {
                    grabs[0].EndManipulation();
                    grabs.RemoveAt(0);
                }

                Assert.That(grabs.Count, Is.LessThanOrEqualTo(2));
            }
        }
    }
}