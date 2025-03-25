using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Nanover.Visualisation.Utility
{
    /// <summary>
    /// Represents the dirty state of an <see cref="IDictionary{TKey,TValue}" />,
    /// keeping track of the keys of items
    /// which have been modified.
    /// </summary>
    public class DictionaryDirtyState<TKey, TValue> : CollectionDirtyState<TKey>
    {
        private readonly IDictionary<TKey, TValue> dictionary;

        public DictionaryDirtyState(IDictionary<TKey, TValue> dictionary) : base(dictionary.Keys)
        {
            this.dictionary = dictionary;
            if (dictionary is INotifyCollectionChanged notify)
                notify.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Collection of dirty keys of the dictionary
        /// </summary>
        public IEnumerable<TKey> DirtyKeys => DirtyItems;

        /// <summary>
        /// Collection of dirty values of the dictionary
        /// </summary>
        public IEnumerable<TValue> DirtyValues => DirtyKeys.Select(key => dictionary[key]);

        /// <summary>
        /// Collection of dirty key-value pairs in the dictionary
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>> DirtyKeyValuePairs =>
            dictionary.Where(kvp => IsDirty(kvp.Key));
    }
}