using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Nanover.Core.Serialization
{
    /// <summary>
    /// A <see cref="JsonWriter" /> for use with json.NET that writes an object to a
    /// JSON-like representation, consisting of <see cref="Dictionary{TKey,TValue}" />,
    /// <see cref="List{Object}" />, <see cref="string" />, <see cref="float" />,
    /// <see cref="string" /> and <see cref="bool" />.
    /// </summary>
    public class CSharpObjectWriter : JsonWriter
    {
        private Stack<object> objects = new Stack<object>();
        private string currentKey;

        public object Object => objects.First();

        /// <summary>
        /// Flushes whatever is in the buffer to the underlying <see cref="JContainer" />.
        /// </summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Writes the beginning of a JSON object.
        /// </summary>
        public override void WriteStartObject()
        {
            base.WriteStartObject();
            AddObjectToStack(new Dictionary<string, object>());
        }

        /// <summary>
        /// Writes the beginning of a JSON array.
        /// </summary>
        public override void WriteStartArray()
        {
            base.WriteStartArray();
            AddObjectToStack(new List<object>());
        }

        private void AddObjectToStack(object container)
        {
            if (objects.Any())
            {
                AddToParent(objects.Peek(), container);
            }

            objects.Push(container);
        }

        private void AddObject(object container)
        {
            if (objects.Any())
            {
                AddToParent(objects.Peek(), container);
            }
            else
            {
                objects.Push(container);
            }
        }
        
        protected override void WriteEnd(JsonToken token)
        {
            if (objects.Count > 1)
                objects.Pop();
        }
        
        private void AddToParent(object parent, object child)
        {
            if (parent is IList<object> list)
                list.Add(child);
            else if (parent is IDictionary<string, object> dict && currentKey != null)
            {
                dict[currentKey] = child;
                currentKey = null;
            }
            else
                throw new ArgumentException($"Cannot parent {child} to {parent}");
        }

        /// <inheritdoc cref="WriteStartConstructor"/>
        public override void WriteStartConstructor(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="WritePropertyName"/>
        public override void WritePropertyName(string name)
        {
            base.WritePropertyName(name);
            currentKey = name;
        }

        private void AddValue(object value)
        {
            AddObject(value);
        }

        /// <inheritdoc cref="WriteNull"/>
        public override void WriteNull()
        {
            base.WriteNull();
            AddValue(null);
        }

        /// <inheritdoc cref="WriteUndefined"/>
        public override void WriteUndefined()
        {
            base.WriteUndefined();
            AddValue(null);
        }

        /// <inheritdoc cref="WriteRaw"/>
        public override void WriteRaw(string json)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="WriteComment"/>
        public override void WriteComment(string text)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(string value)
        {
            base.WriteValue(value);
            AddValue(value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(int value)
        {
            base.WriteValue(value);
            AddValue(value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(uint value)
        {
            base.WriteValue(value);
            AddValue((int) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(long value)
        {
            base.WriteValue(value);
            AddValue((int) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(ulong value)
        {
            base.WriteValue(value);
            AddValue((int) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(float value)
        {
            base.WriteValue(value);
            AddValue((float) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(double value)
        {
            base.WriteValue(value);
            AddValue((float) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(bool value)
        {
            base.WriteValue(value);
            AddValue((bool) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(short value)
        {
            base.WriteValue(value);
            AddValue((int) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(ushort value)
        {
            base.WriteValue(value);
            AddValue((int) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(char value)
        {
            base.WriteValue(value);
            AddValue(value.ToString());
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(byte value)
        {
            base.WriteValue(value);
            AddValue((int) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(sbyte value)
        {
            base.WriteValue(value);
            AddValue((int) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(decimal value)
        {
            base.WriteValue(value);
            AddValue((float) value);
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(DateTime value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(byte[] value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(TimeSpan value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(Guid value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="WriteValue"/>
        public override void WriteValue(Uri value)
        {
            throw new NotSupportedException();
        }
    }
}
