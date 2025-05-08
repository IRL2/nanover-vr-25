using System;
using System.Collections;
using System.Collections.Generic;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Properties.Collections
{
    /// <summary>
    /// Serializable <see cref="Property" /> for an array of <see cref="TValue" />
    /// values;
    /// </summary>
    [Serializable]
    public class ArrayProperty<TValue> : SerializableProperty<TValue[]>, IEnumerable<TValue>
    {
        /// <summary>
        /// Resize the array in this property, creating an array if not possible.
        /// </summary>
        public TValue[] Resize(int size)
        {
            var value = HasValue ? Value : new TValue[0];
            Array.Resize(ref value, size);
            Value = value;
            return Value;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return (Value as IEnumerable<TValue>).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}