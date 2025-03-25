using System;
using Nanover.Visualisation.Node.Spline;
using UnityEngine;

namespace Nanover.Visualisation.Components.Spline
{
    /// <inheritdoc cref="HermiteCurveNode" />
    public class HermiteCurve : VisualisationComponent<HermiteCurveNode>
    {
        private void Update()
        {
            node.Refresh();
        }
    }
}