using UnityEngine;

namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// A property who's value and its presence are both serialized.
    /// </summary>
    public class SerializableProperty<TValue> : LinkableProperty<TValue>
    {
        /// <summary>
        /// Value serialized by Unity.
        /// </summary>
        [SerializeField]
        private TValue value;

        /// <summary>
        /// Override for indicating that the value is null. Unity does not serialize
        /// nullable types, so this is required.
        /// </summary>
        [SerializeField]
        private bool isValueProvided;

        protected override TValue ProvidedValue
        {
            get => value;
            set

            {
                this.value = value;
                isValueProvided = true;
            }
        }

        protected override bool HasProvidedValue => isValueProvided;

        public override void UndefineValue()
        {
            if (HasProvidedValue)
            {
                isValueProvided = false;
                MarkValueAsChanged();
            }

            base.UndefineValue();
        }
    }
}