using System;
using Nanover.Frame;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="BondPair" />
    /// values.
    /// </summary>
    [Serializable]
    public class BondArrayProperty : ArrayProperty<BondPair>
    {
    }
}