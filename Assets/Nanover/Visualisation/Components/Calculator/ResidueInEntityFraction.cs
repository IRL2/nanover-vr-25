using Nanover.Visualisation.Node.Calculator;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ResidueInEntityFractionNode" />
    public class ResidueInEntityFraction : VisualisationComponent<ResidueInEntityFractionNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}