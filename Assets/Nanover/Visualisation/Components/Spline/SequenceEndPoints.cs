using Nanover.Visualisation.Node.Spline;

namespace Nanover.Visualisation.Components.Spline
{
    /// <inheritdoc cref="SequenceEndPointsNode"/>
    public class SequenceEndPoints : VisualisationComponent<SequenceEndPointsNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}