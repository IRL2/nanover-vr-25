using System;

namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// Abstract base class for an implementation of <see cref="IProperty"/>.
    /// </summary>
    [Serializable]
    public abstract class Property : IProperty
    {
        IReadOnlyProperty IProperty.LinkedProperty => NonGenericLinkedProperty;
        
        object IReadOnlyProperty.Value => NonGenericValue;

        protected abstract IReadOnlyProperty NonGenericLinkedProperty { get; }

        protected abstract object NonGenericValue { get; }
        
        /// <summary>
        /// Callback for when the value is changed or undefined.
        /// </summary>
        public event Action ValueChanged;

        /// <summary>
        /// Internal method, called when the value is changed or undefined.
        /// </summary>
        protected void OnValueChanged()
        {
            ValueChanged?.Invoke();
        }

        /// <inheritdoc cref="IReadOnlyProperty.HasValue"/>
        public abstract bool HasValue { get; }

        /// <inheritdoc cref="IProperty.UndefineValue"/>
        public abstract void UndefineValue();

        /// <inheritdoc cref="IProperty.PropertyType"/>
        public abstract Type PropertyType { get; }

        /// <summary>
        /// Has the value of the property changed since the last time the <see cref="IsDirty"/> flag was cleared.
        /// </summary>
        public bool IsDirty { get; set; } = true;

        /// <summary>
        /// Mark the value as having changed.
        /// </summary>
        public virtual void MarkValueAsChanged()
        {
            IsDirty = true;
        }

        /// <inheritdoc cref="IProperty.HasLinkedProperty"/>
        public abstract bool HasLinkedProperty { get; }

        /// <inheritdoc cref="IProperty.TrySetValue"/>
        public abstract void TrySetValue(object value);

        /// <inheritdoc cref="IProperty.TrySetLinkedProperty"/>
        public abstract void TrySetLinkedProperty(object property);
    }
}