using Nanover.Visualisation.Node.Spline;

namespace Nanover.Visualisation.Components.Calculator
{
    /// <inheritdoc cref="CurvedBondNode" />
    public class CurvedBond : VisualisationComponent<CurvedBondNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}