using Nanover.Visualisation.Node.Scale;

namespace Nanover.Visualisation.Components.Scale
{
    /// <inheritdoc cref="VdwScaleNode" />
    public sealed class VdwScale : VisualisationComponent<VdwScaleNode>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            node.Refresh();
        }

        private void Update()
        {
            node.Refresh();
        }
    }
}