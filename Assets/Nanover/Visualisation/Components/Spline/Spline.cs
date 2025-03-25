using Nanover.Visualisation.Node.Spline;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="SplineNode" />
    public class Spline : VisualisationComponent<SplineNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}