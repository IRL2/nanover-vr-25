using Nanover.Visualisation.Node.Adaptor;
using Nanover.Visualisation.Properties;

namespace Nanover.Visualisation.Components.Adaptor
{
    /// <inheritdoc cref="SecondaryStructureAdaptorNode"/>
    public class SecondaryStructureAdaptor : FrameAdaptorComponent<SecondaryStructureAdaptorNode>
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