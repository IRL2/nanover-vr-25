using System;
using Nanover.Visualisation.Components;
using Nanover.Visualisation.Components.Adaptor;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Properties
{
    /// <summary>
    /// Serializable <see cref="Property" /> for a <see cref="IDynamicPropertyProvider" /> value.
    /// </summary>
    [Serializable]
    public class FrameAdaptorProperty : InterfaceProperty<IDynamicPropertyProvider>
    {
    }
}