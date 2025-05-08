using Nanover.Visualisation.Node.Calculator;

namespace Nanover.Visualisation.Components.Spline
{
    public class ColorLerp : VisualisationComponent<ColorLerpNode>
    {
        public void Update()
        {
            node.Refresh();
        }
    }
}