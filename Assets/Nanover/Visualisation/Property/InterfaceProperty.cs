using UnityEngine;

namespace Nanover.Visualisation.Property
{
    /// <summary>
    /// A interface-based property which is serialised as a Unity Object.
    /// </summary>
    public class InterfaceProperty<TValue> : LinkableProperty<TValue>,
                                             ISerializationCallbackReceiver
    {
        /// <summary>
        /// Unity object which could implement the interface.
        /// </summary>
        [SerializeField]
        private Object unityObject;

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
                if (value is Object obj)
                    unityObject = obj;
                else
                    unityObject = null;
                isValueProvided = true;
            }
        }

        protected override bool HasProvidedValue => isValueProvided;

        public override void UndefineValue()
        {
            if (HasProvidedValue)
            {
                isValueProvided = false;
                unityObject = null;
                MarkValueAsChanged();
            }

            base.UndefineValue();
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (unityObject is TValue val)
            {
                this.value = val;
            }
        }
    }
}