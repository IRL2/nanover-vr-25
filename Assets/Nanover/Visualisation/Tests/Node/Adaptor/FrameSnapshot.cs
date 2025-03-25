using Nanover.Frame;
using Nanover.Frame.Event;

namespace Nanover.Visualisation.Tests.Node.Adaptor
{
    internal class FrameSnapshot : ITrajectorySnapshot
    {
        public void Update(Frame.Frame frame, FrameChanges changes)
        {
            CurrentFrame = frame;
            FrameChanged?.Invoke(frame, changes);
        }

        public Frame.Frame CurrentFrame { get; private set; }
        public event FrameChanged FrameChanged;
    }
}