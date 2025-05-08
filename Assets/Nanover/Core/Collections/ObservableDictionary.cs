using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using JetBrains.Annotations;

namespace Nanover.Core.Collections
{
    /// <summary>
    /// A dictionary which implements <see cref="INotifyCollectionChanged" /> on the
    /// keys, such that <see cref="CollectionChanged" /> is invoked when something is
    /// added, removed or updated in the dictionary.
    /// </summary>
    public class ObservableDictionary<TKey, TValue> : INotifyCollectionChanged,
                                                      IDictionary<TKey, TValue>
    {
        [NotNull]
        private readonly IDictionary<TKey, TValue> dictionary;

        public ObservableDictionary([NotNull] IDictionary<TKey, TValue> dictionary)
        {
            this.dictionary = dictionary;
        }

        public ObservableDictionary()
        {
            dictionary = new Dictionary<TKey, TValue>();
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            dictionary.Add(item);
            CollectionChanged?.Invoke(this,
                                      new NotifyCollectionChangedEventArgs(
                                          NotifyCollectionChangedAction.Add, new[] { item.Key }));
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public void Clear()
        {
            var keys = Keys.ToArray();
            dictionary.Clear();
            CollectionChanged?.Invoke(this,
                                      new NotifyCollectionChangedEventArgs(
                                          NotifyCollectionChangedAction.Remove,
                                          keys));
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.Contains(item);
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var removed = dictionary.Remove(item);
            if (removed)
                CollectionChanged?.Invoke(this,
                                          new NotifyCollectionChangedEventArgs(
                                              NotifyCollectionChangedAction.Remove,
                                              new[] { item.Key }));
            return removed;
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public int Count => dictionary.Count;

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool IsReadOnly => dictionary.IsReadOnly;

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
            CollectionChanged?.Invoke(this,
                                      new NotifyCollectionChangedEventArgs(
                                          NotifyCollectionChangedAction.Add, new[] { key }));
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool Remove(TKey key)
        {
            var result = dictionary.Remove(key);
            CollectionChanged?.Invoke(this,
                                      new NotifyCollectionChangedEventArgs(
                                          NotifyCollectionChangedAction.Remove, new[] { key }));
            return result;
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public TValue this[TKey key]
        {
            get => dictionary[key];
            set
            {
                var existingKey = ContainsKey(key);
                dictionary[key] = value;
                if (existingKey)
                    CollectionChanged?.Invoke(this,
                                              new NotifyCollectionChangedEventArgs(
                                                  NotifyCollectionChangedAction.Replace,
                                                  new[] { key },
                                                  new[] { key }));
                else
                    CollectionChanged?.Invoke(this,
                                              new NotifyCollectionChangedEventArgs(
                                                  NotifyCollectionChangedAction.Add,
                                                  new[] { key }));
            }
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public ICollection<TKey> Keys => dictionary.Keys;

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public ICollection<TValue> Values => dictionary.Values;

        /// <inheritdoc cref="INotifyCollectionChanged" />
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}