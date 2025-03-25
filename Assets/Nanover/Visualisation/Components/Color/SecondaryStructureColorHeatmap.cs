using Nanover.Visualisation.Node.Color;

namespace Nanover.Visualisation.Components.Color
{
    /// <inheritdoc cref="Node.Color.SecondaryStructureColorHeatmap" />
    public class SecondaryStructureColorHeatmap:
        VisualisationComponent<Node.Color.SecondaryStructureColorHeatmap>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}