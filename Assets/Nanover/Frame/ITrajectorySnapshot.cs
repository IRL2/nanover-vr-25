using System;
using Nanover.Frame.Event;

namespace Nanover.Frame
{
    public delegate void FrameChanged(IFrame frame, FrameChanges changes);
        
    /// <summary>
    /// A single <see cref="Frame" /> in a trajectory, representing a single point in
    /// time. Use <see cref="FrameChanged" /> for a
    /// </summary>
    public interface ITrajectorySnapshot
    {
        /// <summary>
        /// Current frame.
        /// </summary>
        Frame CurrentFrame { get; }

        /// <summary>
        /// Event invoked when the frame is changed.
        /// </summary>
        event FrameChanged FrameChanged;
    }
}