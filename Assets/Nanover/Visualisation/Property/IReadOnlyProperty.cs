using System;

namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// Properties allow the linking together of inputs and outputs of various objects
    /// in an observer pattern. Read-only properties are value providers, whilst
    /// <see cref="IProperty" /> can also be linked to other properties to
    /// obtain their values.
    /// </summary>
    public interface IReadOnlyProperty
    {
        /// <summary>
        /// The current value of the property. This should only be called if <see cref="HasValue"/> has been ensured to be true.
        /// </summary>
        object Value { get; }
        
        /// <summary>
        /// Does the property currently have a value? If true, then accessing <see cref="Value"/> should be valid.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Callback for when the value is changed.
        /// </summary>
        event Action ValueChanged;
        
        /// <summary>
        /// The <see cref="Type"/> that this property wraps.
        /// </summary>
        Type PropertyType { get; }
    }
    
    /// <inheritdoc cref="IReadOnlyProperty"/>
    public interface IReadOnlyProperty<out TValue> : IReadOnlyProperty
    {
        /// <inheritdoc cref="IReadOnlyProperty.Value"/>
        new TValue Value { get; }
    }
}