namespace Nanover.Frame
{
    /// <summary>
    /// Represents something that can consume a source of <see cref="IFrame" />.
    /// </summary>
    public interface IFrameConsumer
    {
        /// <summary>
        /// The source of <see cref="IFrame" /> to use.
        /// </summary>
        ITrajectorySnapshot FrameSource { set; }
    }
}