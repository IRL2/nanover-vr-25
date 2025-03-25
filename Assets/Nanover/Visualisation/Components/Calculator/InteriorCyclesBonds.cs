using Nanover.Visualisation.Node.Calculator;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="InteriorCyclesBondsNode" />
    public class InteriorCyclesBonds : VisualisationComponent<InteriorCyclesBondsNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}