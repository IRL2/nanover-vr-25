using Nanover.Visualisation.Node.Calculator;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ParticleInSystemFractionNode" />
    public class ParticleInSystemFraction : VisualisationComponent<ParticleInSystemFractionNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}