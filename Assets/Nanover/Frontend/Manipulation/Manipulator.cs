using System;
using Nanover.Core.Math;
using Nanover.Frontend.Input;

namespace Nanover.Frontend.Manipulation
{
    /// <summary>
    /// Represents an input device posed in 3D space (e.g a VR controller) that can
    /// engage in a single manipulation.
    /// </summary>
    public class Manipulator : IPosedObject
    {
        public Transformation? Pose => posedObject.Pose;
        public event Action PoseChanged;

        private readonly IPosedObject posedObject;
        private IActiveManipulation activeManipulation;

        public Manipulator(IPosedObject posedObject)
        {
            this.posedObject = posedObject;
            posedObject.PoseChanged += UpdatePose;
        }

        /// <summary>
        /// Set the active manipulation, ending any existing manipulation first.
        /// </summary>
        public void SetActiveManipulation(IActiveManipulation manipulation)
        {
            EndActiveManipulation();

            activeManipulation = manipulation;
        }

        /// <summary>
        /// End the active manipulation if there is any.
        /// </summary>
        public void EndActiveManipulation()
        {
            activeManipulation?.EndManipulation();
            activeManipulation = null;
        }

        private void UpdatePose()
        {
            if (posedObject.Pose is Transformation pose)
            {
                activeManipulation?.UpdateManipulatorPose(pose.AsUnitTransformWithoutScale());
            }
            else
            {
                EndActiveManipulation();
            }

            PoseChanged?.Invoke();
        }
    }
}