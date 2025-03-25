using Nanover.Visualisation.Node.Filter;

namespace Nanover.Visualisation.Components.Filter
{
    /// <inheritdoc cref="ResidueNameFilterNode" />
    public sealed class ResidueNameFilter : VisualisationComponent<ResidueNameFilterNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}