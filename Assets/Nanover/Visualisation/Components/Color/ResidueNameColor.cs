using Nanover.Visualisation.Node.Color;

namespace Nanover.Visualisation.Components.Color
{
    /// <inheritdoc cref="ResidueNameColorNode" />
    public class ResidueNameColor :
        VisualisationComponent<ResidueNameColorNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}