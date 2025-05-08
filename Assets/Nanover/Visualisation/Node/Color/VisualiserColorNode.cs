using System;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    /// <summary>
    /// Base code for a visualiser node which generates a set of colors.
    /// </summary>
    [Serializable]
    public abstract class VisualiserColorNode : GenericOutputNode
    {
        protected readonly ColorArrayProperty colors = new ColorArrayProperty();

        /// <summary>
        /// Color array output.
        /// </summary>
        public IReadOnlyProperty<UnityEngine.Color[]> Colors => colors;
    }
}