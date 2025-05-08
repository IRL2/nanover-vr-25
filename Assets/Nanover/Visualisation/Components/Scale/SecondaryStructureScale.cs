namespace Nanover.Visualisation.Components.Scale
{
    /// <inheritdoc cref="Node.Scale.SecondaryStructureScaleNode" />
    public sealed class SecondaryStructureScale : VisualisationComponent<Node.Scale.SecondaryStructureScaleNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}