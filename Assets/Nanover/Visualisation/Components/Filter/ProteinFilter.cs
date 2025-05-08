using Nanover.Visualisation.Node.Filter;

namespace Nanover.Visualisation.Components.Filter
{
    /// <inheritdoc cref="ProteinFilterNode" />
    public sealed class ProteinFilter : VisualisationComponent<ProteinFilterNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}