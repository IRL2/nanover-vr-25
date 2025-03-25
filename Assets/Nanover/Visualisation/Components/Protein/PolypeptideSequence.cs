using Nanover.Visualisation.Node.Protein;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="PolypeptideSequenceNode" />
    public class PolypeptideSequence : VisualisationComponent<PolypeptideSequenceNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}