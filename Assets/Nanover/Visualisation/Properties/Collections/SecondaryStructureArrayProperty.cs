using System;
using Nanover.Visualisation.Node.Protein;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="SecondaryStructureAssignment" /> values.
    /// </summary>
    [Serializable]
    public class SecondaryStructureArrayProperty : ArrayProperty<SecondaryStructureAssignment>
    {
    }
}