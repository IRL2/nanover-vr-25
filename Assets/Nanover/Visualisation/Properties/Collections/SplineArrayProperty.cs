using System;
using Nanover.Visualisation.Node.Spline;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="SplineSegment" /> values.
    /// </summary>
    [Serializable]
    public class SplineArrayProperty : ArrayProperty<SplineSegment>
    {
    }
}