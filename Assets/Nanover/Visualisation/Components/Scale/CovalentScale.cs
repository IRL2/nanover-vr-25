using Nanover.Visualisation.Node.Scale;

namespace Nanover.Visualisation.Components.Scale
{
    /// <inheritdoc cref="CovalentScaleNode" />
    public sealed class CovalentScale : VisualisationComponent<CovalentScaleNode>
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