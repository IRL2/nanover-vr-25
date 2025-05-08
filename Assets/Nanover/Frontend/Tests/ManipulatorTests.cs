using Nanover.Core.Math;
using Nanover.Frontend.Input;
using Nanover.Frontend.Manipulation;
using NUnit.Framework;
using System;
using UnityEngine;

namespace Nanover.Frontend.Tests
{
    internal class ManipulatorTests
    {
        [Test]
        public void PoseChanged_WithSingleChangeOnSourcePosedObject_InvokedAfterSetPoseOn()
        {
            var poser = new DirectPosedObject();
            var manipulator = new Manipulator(poser);

            var wasChanged = false;
            manipulator.PoseChanged += () => wasChanged = true;
            poser.SetPose(null);

            Assert.IsTrue(wasChanged);
        }

        [Test]
        public void SetActiveManipulation_WithNullManipulation_EndsPreviousManipulation()
        {
            var poser = new DirectPosedObject();
            var manipulator = new Manipulator(poser);

            var manipulation = new FakeManipulation();

            manipulator.SetActiveManipulation(manipulation);
            manipulator.SetActiveManipulation(null);

            Assert.IsTrue(manipulation.Ended);
        }

        [Test]
        public void SetObjectPose_WithIdentity_UpdatesPoseOnManipulation()
        {
            var poser = new DirectPosedObject();
            var manipulator = new Manipulator(poser);

            var manipulation = new FakeManipulation();

            manipulation.LastPose = null;
            manipulator.SetActiveManipulation(manipulation);
            poser.SetPose(Transformation.Identity);

            Assert.IsTrue(manipulation.LastPose.Value.Matrix == Matrix4x4.identity);
        }

        [Test]
        public void SetObjectPose_WithNullPose_EndsActiveManipulation()
        {
            var poser = new DirectPosedObject();
            var manipulator = new Manipulator(poser);

            var manipulation = new FakeManipulation();

            manipulator.SetActiveManipulation(manipulation);
            poser.SetPose(null);

            Assert.IsTrue(manipulation.Ended);
        }

        [Test]
        public void SetObjectPose_WithEndedManipulation_DoesNotUpdateManipulationPose()
        {
            var poser = new DirectPosedObject();
            var manipulator = new Manipulator(poser);

            var manipulation = new FakeManipulation();

            manipulation.LastPose = null;
            manipulator.SetActiveManipulation(manipulation);
            manipulator.EndActiveManipulation();
            poser.SetPose(Transformation.Identity);

            Assert.IsNull(manipulation.LastPose);
        }

        [Test]
        public void EndActiveManipulation_CallsEndsManipulationOnManipulation()
        {
            var poser = new DirectPosedObject();
            var manipulator = new Manipulator(poser);

            var manipulation = new FakeManipulation();

            manipulator.SetActiveManipulation(manipulation);
            manipulator.EndActiveManipulation();

            Assert.IsTrue(manipulation.Ended);
        }

        [Test]
        public void BindButton_WithButtonPress_SetsActiveManipulation()
        {
            var poser = new DirectPosedObject();
            var button = new DirectButton();
            var manipulation = new FakeManipulation();
            var manipulator = new AttemptableManipulator(poser, 
                                                         pose => manipulation);
            button.Pressed += manipulator.AttemptManipulation;

            poser.SetPose(Transformation.Identity);

            button.Press();
            manipulator.EndActiveManipulation();

            Assert.IsTrue(manipulation.Ended);
        }

        private class FakeManipulation : IActiveManipulation
        {
            public event Action ManipulationEnded; 

            public bool Ended = false;
            public Transformation? LastPose;

            public void UpdateManipulatorPose(UnitScaleTransformation manipulatorPose)
            {
                LastPose = manipulatorPose;
            }

            public void EndManipulation()
            {
                Ended = true;
                ManipulationEnded?.Invoke();
            }
        }
    }
}