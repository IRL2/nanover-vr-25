using Nanover.Visualisation.Node.Spline;

namespace Nanover.Visualisation.Components.Spline
{
    /// <inheritdoc cref="TetrahedralSplineNode" />
    public class TetrahedralSpline : VisualisationComponent<TetrahedralSplineNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}