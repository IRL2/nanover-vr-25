using Nanover.Visualisation.Node.Calculator;

namespace Nanover.Visualisation.Components.Spline
{
    public class FloatLerp : VisualisationComponent<FloatLerpNode>
    {
        public void Update()
        {
            node.Refresh();
        }
    }
}