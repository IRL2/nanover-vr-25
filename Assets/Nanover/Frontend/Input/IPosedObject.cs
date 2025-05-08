using System;
using Nanover.Core.Math;

namespace Nanover.Frontend.Input
{
    /// <summary>
    /// Represents an object posed in 3D space.
    /// </summary>
    public interface IPosedObject
    {
        /// <summary>
        /// The pose of the object, if available.
        /// </summary>
        Transformation? Pose { get; }

        /// <summary>
        /// Occurs when the object's pose changes.
        /// </summary>
        event Action PoseChanged;
    }
}