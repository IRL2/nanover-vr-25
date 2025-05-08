using JetBrains.Annotations;
using Nanover.Frame;
using Nanover.Frame.Event;
using UnityEngine;

namespace Nanover.Visualisation
{
    /// <summary>
    /// Interface between a <see cref="ITrajectorySnapshot" /> and Unity. Frontend
    /// specific tasks such as rendering should utilise this to track the frame, as it
    /// delays frame updating to the main thread.
    /// </summary>
    [DisallowMultipleComponent]
    public class SynchronisedFrameSource : MonoBehaviour,
                                           ITrajectorySnapshot,
                                           IFrameConsumer
    {
        /// <inheritdoc cref="ITrajectorySnapshot.CurrentFrame" />
        public Frame.Frame CurrentFrame => snapshot?.CurrentFrame;

        /// <inheritdoc cref="ITrajectorySnapshot.FrameChanged" />
        public event FrameChanged FrameChanged;

        private ITrajectorySnapshot snapshot;

        /// <summary>
        /// Source for the frames to be displayed.
        /// </summary>
        public ITrajectorySnapshot FrameSource
        {
            set
            {
                if (snapshot != null)
                    snapshot.FrameChanged -= SnapshotOnFrameChanged;
                snapshot = value;
                if (snapshot != null)
                {
                    snapshot.FrameChanged += SnapshotOnFrameChanged;
                    changes = FrameChanges.All;
                }
            }
        }

        /// <summary>
        /// Callback when a frame is updated, which can happen outwith the main Unity
        /// thread.
        /// </summary>
        private void SnapshotOnFrameChanged(IFrame frame, FrameChanges changes)
        {
            this.changes.MergeChanges(changes);
        }

        [NotNull]
        private FrameChanges changes = FrameChanges.None;

        private void Update()
        {
            FlushChanges();
        }

        /// <summary>
        /// Raise <see cref="FrameChanged" /> if necessary then clear state to
        /// unchanged.
        /// </summary>
        private void FlushChanges()
        {
            if (changes.HasAnythingChanged)
            {
                FrameChanged?.Invoke(snapshot.CurrentFrame, changes);
                changes = FrameChanges.None;
            }
        }
    }
}