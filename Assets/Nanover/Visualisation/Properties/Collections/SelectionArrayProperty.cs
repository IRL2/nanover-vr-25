using System;
using System.Collections.Generic;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="int" /> values.
    /// </summary>
    [Serializable]
    public class SelectionArrayProperty : ArrayProperty<IReadOnlyList<int>>
    {
    }
}