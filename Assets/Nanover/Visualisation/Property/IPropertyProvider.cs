using System.Collections.Generic;

namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// Describes something which provides access to visualisation properties.
    /// </summary>
    /// <remarks>
    /// See <seealso cref="IReadOnlyProperty" /> for more information.
    /// </remarks>
    public interface IPropertyProvider
    {
        /// <summary>
        /// Get a property which exists with the given name. Returns null if the property
        /// with a given name is null.
        /// </summary>
        IReadOnlyProperty GetProperty(string name);

        /// <summary>
        /// Get all properties and their associated keys that exist on this object.
        /// </summary>
        IEnumerable<(string name, IReadOnlyProperty property)> GetProperties();
    }
}