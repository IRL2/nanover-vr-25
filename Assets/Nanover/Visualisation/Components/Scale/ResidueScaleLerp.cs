using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nanover.Visualisation.Components.Scale
{
    /// <inheritdoc cref="Node.Scale.ResidueScaleLerp" />
    public sealed class ResidueScaleLerp : VisualisationComponent<Node.Scale.ResidueScaleLerp>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}