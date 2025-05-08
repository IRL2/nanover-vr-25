using System;
using Nanover.Core.Math;

namespace Nanover.Frontend.Manipulation
{
    /// <summary>
    /// A handle for communicating updates from a manipulator (e.g XR controller)
    /// to the manipulation it is actively engaging in (e.g grabbing an object)
    /// </summary>
    public interface IActiveManipulation
    {
        /// <summary>
        /// Raised once when the manipulation ends.
        /// </summary>
        event Action ManipulationEnded;

        /// <summary>
        /// Provide an updated pose transformation for the manipulator.
        /// </summary>
        void UpdateManipulatorPose(UnitScaleTransformation manipulatorPose);

        /// <summary>
        /// End the manipulation, and expect no further relationship between the
        /// manipulator and this manipulation.
        /// </summary>
        void EndManipulation();
    }
}