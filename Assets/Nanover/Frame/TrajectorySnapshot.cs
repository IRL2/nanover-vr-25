using System;
using Nanover.Frame.Event;

namespace Nanover.Frame
{
    /// <summary>
    /// Maintains a single <see cref="Frame" />, which is updated by receiving new data.
    /// When updated, old fields are maintained except when replaced by data in the
    /// previous frame.
    /// </summary>
    public class TrajectorySnapshot : ITrajectorySnapshot
    {
        /// <inheritdoc cref="ITrajectorySnapshot.CurrentFrame" />
        public Frame CurrentFrame { get; private set; }

        public FrameChanges CurrentFrameChanges { get; private set; }

        /// <summary>
        /// Set the current frame to completely empty, with every field changed.
        /// </summary>
        public void Clear() => SetCurrentFrame(new Frame(), FrameChanges.All);

        /// <summary>
        /// Set the current frame, replacing the existing one.
        /// </summary>
        public void SetCurrentFrame(Frame frame, FrameChanges changes)
        {
            CurrentFrame = frame;
            CurrentFrameChanges = changes;
            FrameChanged?.Invoke(CurrentFrame, CurrentFrameChanges);
        }

        /// <inheritdoc cref="ITrajectorySnapshot.FrameChanged" />
        public event FrameChanged FrameChanged;
    }
}