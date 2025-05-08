using System;
using JetBrains.Annotations;

namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// Implementation of a property which does not define if it itself provides a value, but allows linking to other properties.
    /// </summary>
    public abstract class LinkableProperty<TValue> : Property, IProperty<TValue>
    {
        /// <summary>
        /// A linked <see cref="Property" /> which can provide a value.
        /// </summary>
        [CanBeNull]
        private IReadOnlyProperty<TValue> linkedProperty;

        protected override IReadOnlyProperty NonGenericLinkedProperty => LinkedProperty;

        protected override object NonGenericValue => Value;
        
        /// <inheritdoc cref="IProperty{TValue}.HasValue" />
        public override bool HasValue
        {
            get
            {
                if (!isCachedHasValueValid)
                {
                    cachedHasValue = GetHasValueInternal();
                    isCachedHasValueValid = true;
                }

                return cachedHasValue;
            }
        }

        private bool isCachedValueValid = false;
        private bool isCachedHasValueValid = false;
        private bool cachedHasValue = false;
        private TValue cachedValue = default;

        private void ResetCache()
        {
            isCachedValueValid = false;
            cachedValue = default;
            isCachedHasValueValid = false;
            cachedHasValue = false;
        }

        private bool GetHasValueInternal()
        {
            if (HasLinkedProperty)
                return LinkedProperty.HasValue;
            if (HasProvidedValue)
                return true;
            return false;
        }

        /// <inheritdoc cref="Property.MarkValueAsChanged" />
        public override void MarkValueAsChanged()
        {
            ResetCache();
            base.MarkValueAsChanged();
            OnValueChanged();
        }

        protected abstract TValue ProvidedValue { get; set; }

        protected abstract bool HasProvidedValue { get; }

        /// <inheritdoc cref="IProperty{TValue}.Value" />
        public virtual TValue Value
        {
            get
            {
                if (!isCachedValueValid)
                {
                    cachedValue = GetValueInternal();
                    isCachedValueValid = true;
                }

                return cachedValue;
            }
            set
            {
                LinkedProperty = null;
                ProvidedValue = value;
                MarkValueAsChanged();
            }
        }

        private TValue GetValueInternal()
        {
            if (HasLinkedProperty)
                return LinkedProperty.Value;
            if (HasProvidedValue)
                return ProvidedValue;
            throw new InvalidOperationException(
                "Tried accessing value of property when it is not defined");
        }

        /// <inheritdoc cref="IProperty{TValue}.HasLinkedProperty" />
        public override bool HasLinkedProperty => LinkedProperty != null;

        /// <inheritdoc cref="IProperty{TValue}.LinkedProperty" />
        [CanBeNull]
        public IReadOnlyProperty<TValue> LinkedProperty
        {
            get => linkedProperty;
            set
            {
                if (linkedProperty == value)
                    return;

                if (value == this)
                    throw new ArgumentException("Cannot link property to itself!");

                // Check no cyclic linked properties will occur
                var linked = value is SerializableProperty<TValue> linkable
                                 ? linkable.LinkedProperty
                                 : null;
                while (linked != null)
                {
                    if (linked == this)
                        throw new ArgumentException("Cyclic link detected!");
                    linked = linked is SerializableProperty<TValue> linkable2
                                 ? linkable2.LinkedProperty
                                 : null;
                }


                if (linkedProperty != null)
                    linkedProperty.ValueChanged -= MarkValueAsChanged;
                linkedProperty = value;
                if (linkedProperty != null)
                    linkedProperty.ValueChanged += MarkValueAsChanged;

                MarkValueAsChanged();
            }
        }

        /// <inheritdoc cref="IProperty{TValue}.UndefineValue" />
        public override void UndefineValue()
        {
            LinkedProperty = null;
            MarkValueAsChanged();
        }

        /// <summary>
        /// Implicit conversion of the property to its value
        /// </summary>
        public static implicit operator TValue(LinkableProperty<TValue> property)
        {
            return property.Value;
        }

        /// <inheritdoc cref="Property.PropertyType" />
        public override Type PropertyType => typeof(TValue);

        /// <inheritdoc cref="Property.TrySetValue" />
        public override void TrySetValue(object value)
        {
            if (value is TValue validValue)
                Value = validValue;
            else if (value == default)
                Value = default;
            // Check if the `value` is a double and `TValue` is a float (single) if so then cast
            // `value` from type double to type float (single) and assign. This hack exists to fix
            // the issue with trying to send single float precision values from the python server.
            // In theory the only time that this would break is if there are two entities of the
            // same name present in the graph but with differing data types. However, such an
            // eventuality is unlikely and would result in a crash anyway. Thus, this hack should,
            // at least in theory, not pose a problem.
            else if (value is double asDouble && typeof(TValue) == typeof(float))
                Value = (TValue)(object)(float)asDouble;
            else
                throw new ArgumentException($"Tried to set property of type {typeof(TValue)} to value '{value}' of type {value.GetType()}.");
        }

        /// <inheritdoc cref="Property.TrySetLinkedProperty" />
        public override void TrySetLinkedProperty(object value)
        {
            if (value is IReadOnlyProperty<TValue> validValue)
                LinkedProperty = validValue;
            else if (value == null)
                LinkedProperty = null;
            else
                throw new ArgumentException($"Cannot set linked property {value} for {this}");
        }
    }
}