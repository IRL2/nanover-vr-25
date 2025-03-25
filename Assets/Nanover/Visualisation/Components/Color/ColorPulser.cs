using Nanover.Visualisation.Node.Color;

namespace Nanover.Visualisation.Components.Color
{
    /// <inheritdoc cref="ColorPulserNode" />
    public sealed class ColorPulser : VisualisationComponent<ColorPulserNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}