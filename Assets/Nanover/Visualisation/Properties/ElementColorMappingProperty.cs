using System;
using Nanover.Core;
using Nanover.Core.Science;
using Nanover.Visualisation.Node.Color;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Properties
{
    /// <summary>
    /// Serializable <see cref="Property" /> for a <see cref="ElementColorMapping" />
    /// value.
    /// </summary>
    [Serializable]
    public class ElementColorMappingProperty : InterfaceProperty<IMapping<Element, Color>>
    {
    }
}