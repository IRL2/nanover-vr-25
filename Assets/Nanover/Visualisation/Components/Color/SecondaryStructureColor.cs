using Nanover.Visualisation.Node.Color;

namespace Nanover.Visualisation.Components.Color
{
    /// <inheritdoc cref="Node.Color.SecondaryStructureColor" />
    public class SecondaryStructureColor :
        VisualisationComponent<Node.Color.SecondaryStructureColor>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}