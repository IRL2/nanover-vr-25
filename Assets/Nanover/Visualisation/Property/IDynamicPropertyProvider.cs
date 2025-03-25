using System;
using System.Collections.Generic;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Components
{
    public interface IDynamicPropertyProvider : IPropertyProvider
    {
        /// <summary>
        /// Get an existing property, or attempt to dynamically create one with the given
        /// type.
        /// </summary>
        IReadOnlyProperty<T> GetOrCreateProperty<T>(string name);

        /// <summary>
        /// Get a list of potential properties, some of which may already exist.
        /// </summary>
        /// <remarks>
        /// A returned item of this method indicates that
        /// <see cref="IDynamicPropertyProvider.GetOrCreateProperty{T}" /> will be successful with the given name
        /// and type. However, that method may also support arbitary names/types
        /// depending on the implementation.
        /// </remarks>
        IEnumerable<(string name, Type type)> GetPotentialProperties();
        
        /// <summary>
        /// Could this provider give a property on a call to
        /// <see cref="IDynamicPropertyProvider.GetOrCreateProperty{T}" />?.
        /// </summary>
        bool CanDynamicallyProvideProperty<T>(string name);
    }
}