using Nanover.Visualisation.Node.Adaptor;

namespace Nanover.Visualisation.Components.Adaptor
{
    /// <inheritdoc cref="ParticleFilteredAdaptorNode"/>
    public class ParticleFilteredAdaptor : FrameAdaptorComponent<ParticleFilteredAdaptorNode>
    {
        protected override void OnDisable()
        {
            base.OnDisable();

            // Unlink the adaptor, preventing memory leaks
            node.ParentAdaptor.UndefineValue();
            node.Refresh();
        }
    }
}