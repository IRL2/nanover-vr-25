using Nanover.Visualisation.Node.Protein;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ByEntitySequenceNode" />
    public class ByEntitySequence : VisualisationComponent<ByEntitySequenceNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}