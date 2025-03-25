using Nanover.Visualisation.Node.Spline;

namespace Nanover.Visualisation.Components.Spline
{
    /// <inheritdoc cref="NormalOrientationNode" />
    public class NormalOrientation : VisualisationComponent<NormalOrientationNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}