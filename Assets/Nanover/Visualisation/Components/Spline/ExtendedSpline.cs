using Nanover.Visualisation.Node.Spline;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="ExtendedSplineNode" />
    public class ExtendedSpline : VisualisationComponent<ExtendedSplineNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}