using System;
using System.Collections.Generic;
using Nanover.Core.Math;
using UnityEngine;

namespace Nanover.Frontend.Manipulation
{
    /// <summary>
    /// Wrapper around a Unity Transform that allows up to two grab manipulations
    /// to translate, rotate, and scale it.
    /// </summary>
    public class ManipulableTransform
    {
        /// <summary>
        /// Represents an active grab manipulation, storing updates to the
        /// world pose of the grabber (e.g controller)
        /// </summary>
        private class GrabManipulation : IActiveManipulation
        {
            public event Action ManipulationEnded;
            public UnitScaleTransformation ManipulatorPose { get; private set; }

            private readonly ManipulableTransform Manipulable;

            public GrabManipulation(ManipulableTransform manipulable)
            {
                Manipulable = manipulable;
            }

            public void UpdateManipulatorPose(UnitScaleTransformation manipulatorPose)
            {
                ManipulatorPose = manipulatorPose;
                Manipulable.UpdateGesturesFromActiveManipulations();
            }

            public void EndManipulation()
            {
                Manipulable.EndGrabManipulation(this);
                ManipulationEnded?.Invoke();
            }
        }

        private readonly OnePointTranslateRotateGesture OnePointGesture
            = new OnePointTranslateRotateGesture();

        private readonly TwoPointTranslateRotateScaleGesture TwoPointGesture
            = new TwoPointTranslateRotateScaleGesture();

        private readonly List<GrabManipulation> manipulations = new List<GrabManipulation>();

        private readonly Transform transform;

        public ManipulableTransform(Transform transform)
        {
            this.transform = transform;
        }

        /// <summary>
        /// Start an additional grab manipulation on this object using the
        /// given pose of the manipulator (e.g controller), returning an object
        /// which accepts updates to the manipulator pose.
        /// A second grab will end the one-point gesture and begin a new
        /// two-point gesture.
        /// </summary>
        public IActiveManipulation StartGrabManipulation(UnitScaleTransformation manipulatorPose)
        {
            if (manipulations.Count == 0)
            {
                return StartFirstGrab(manipulatorPose);
            }
            else if (manipulations.Count == 1)
            {
                return StartSecondGrab(manipulatorPose);
            }

            return null;
        }

        private IActiveManipulation StartFirstGrab(UnitScaleTransformation manipulatorPose)
        {
            var manipulation = CreateManipulation(manipulatorPose);

            OnePointGesture.BeginGesture(Transformation.FromTransformRelativeToWorld(transform),
                                         manipulatorPose);

            return manipulation;
        }

        private IActiveManipulation StartSecondGrab(UnitScaleTransformation manipulatorPose)
        {
            var manipulation = CreateManipulation(manipulatorPose);
            var pose0 = manipulations[0].ManipulatorPose;
            var pose1 = manipulations[1].ManipulatorPose;

            TwoPointGesture.BeginGesture(Transformation.FromTransformRelativeToWorld(transform),
                                         pose0,
                                         pose1);

            return manipulation;
        }

        private IActiveManipulation CreateManipulation(UnitScaleTransformation manipulatorPose)
        {
            var manipulation = new GrabManipulation(this);
            manipulation.UpdateManipulatorPose(manipulatorPose);

            manipulations.Add(manipulation);

            return manipulation;
        }

        /// <summary>
        /// End the given grab manipulation. If there is still an active grab
        /// manipulation then the two-point gesture will end and a new one-point
        /// gesture will begin.
        /// </summary>
        private void EndGrabManipulation(GrabManipulation manipulation)
        {
            manipulations.Remove(manipulation);

            if (manipulations.Count == 1)
            {
                SwitchToSinglePointManipulation();
            }
        }

        private void SwitchToSinglePointManipulation()
        {
            var pose = manipulations[0].ManipulatorPose;
            var transformation = Transformation.FromTransformRelativeToWorld(transform);

            OnePointGesture.BeginGesture(transformation, pose);
        }

        /// <summary>
        /// Integrate updates to the grab points into the active gesture and
        /// update the transform to match.
        /// </summary>
        public void UpdateGesturesFromActiveManipulations()
        {
            if (manipulations.Count == 1)
            {
                UpdateOnePointManipulation();
            }
            else if (manipulations.Count == 2)
            {
                UpdateTwoPointManipulation();
            }
        }

        private void UpdateOnePointManipulation()
        {
            var pose = manipulations[0].ManipulatorPose;
            var transformation = OnePointGesture.UpdateControlPoint(pose);

            transformation.CopyToTransformRelativeToWorld(transform);
        }

        private void UpdateTwoPointManipulation()
        {
            var pose0 = manipulations[0].ManipulatorPose;
            var pose1 = manipulations[1].ManipulatorPose;

            var transformation = TwoPointGesture.UpdateControlPoints(pose0, pose1);

            transformation.CopyToTransformRelativeToWorld(transform);
        }
    }
}