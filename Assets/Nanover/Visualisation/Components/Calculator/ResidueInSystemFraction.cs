using Nanover.Visualisation.Node.Calculator;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ResidueInSystemFractionNode" />
    public class ResidueInSystemFraction : VisualisationComponent<ResidueInSystemFractionNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}