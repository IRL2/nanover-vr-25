using Nanover.Frame;
using Nanover.Visualisation.Node.Adaptor;

namespace Nanover.Visualisation.Components.Adaptor
{
    /// <inheritdoc cref="FrameAdaptorNode" />
    public sealed class FrameAdaptor : FrameAdaptorComponent<FrameAdaptorNode>, IFrameConsumer
    {
        /// <inheritdoc cref="IFrameConsumer.FrameSource" />
        public ITrajectorySnapshot FrameSource
        {
            set => node.FrameSource = value;
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            
            // Unlink the adaptor, preventing memory leaks
            node.FrameSource = null;
            node.Refresh();
        }
    }
}