using Nanover.Visualisation.Node.Spline;

namespace Nanover.Visualisation.Components.Spline
{
    /// <inheritdoc cref="PolypeptideCurveNode" />
    public class PolypeptideCurve : VisualisationComponent<PolypeptideCurveNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}