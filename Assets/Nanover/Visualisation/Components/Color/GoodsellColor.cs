using Nanover.Visualisation.Node.Color;

namespace Nanover.Visualisation.Components.Color
{
    /// <inheritdoc cref="GoodsellColorNode" />
    public sealed class GoodsellColor : VisualisationComponent<GoodsellColorNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}