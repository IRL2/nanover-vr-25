using Nanover.Visualisation.Node.Protein;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="SecondaryStructureNode" />
    public class SecondaryStructure : VisualisationComponent<SecondaryStructureNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}