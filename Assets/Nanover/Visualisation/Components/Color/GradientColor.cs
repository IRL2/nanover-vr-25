using Nanover.Visualisation.Node.Color;

namespace Nanover.Visualisation.Components.Color
{
    /// <inheritdoc cref="GradientColorNode" />
    public sealed class GradientColor : VisualisationComponent<GradientColorNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}