using Nanover.Visualisation.Node.Calculator;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="CyclesCalculatorNode" />
    public class CyclesCalculator : VisualisationComponent<CyclesCalculatorNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}