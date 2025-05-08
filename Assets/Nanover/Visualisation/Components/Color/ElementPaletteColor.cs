using Nanover.Visualisation.Node.Color;

namespace Nanover.Visualisation.Components.Color
{
    /// <inheritdoc cref="ElementColorMappingNode" />
    public sealed class ElementPaletteColor : VisualisationComponent<ElementColorMappingNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}