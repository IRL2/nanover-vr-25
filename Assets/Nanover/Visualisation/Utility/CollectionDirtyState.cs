using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using JetBrains.Annotations;

namespace Nanover.Visualisation.Utility
{
    /// <summary>
    /// Tracks the dirty state of an <see cref="IEnumerable" />, by tracking items
    /// which have changed since the last
    /// time the dirty state of this object was reset. If the provided collection
    /// implements
    /// <see cref="INotifyCollectionChanged" />, it will automatically use
    /// <see cref="INotifyCollectionChanged.CollectionChanged" /> to update the dirty
    /// state.
    /// </summary>
    /// <remarks>
    /// The dirty items are stored in no particular order.
    /// </remarks>
    public class CollectionDirtyState<T> : IReadOnlyDictionary<T, bool>
    {
        private readonly HashSet<T> dirtyItems = new HashSet<T>();

        private readonly ICollection<T> collection;

        public CollectionDirtyState([NotNull] ICollection<T> collection)
        {
            this.collection = collection;

            foreach (var item in collection)
                MarkDirty(item);

            if (collection is INotifyCollectionChanged notify)
                notify.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Collection of dirty items.
        /// </summary>
        public IReadOnlyCollection<T> DirtyItems => dirtyItems;

        /// <inheritdoc cref="IEnumerable{T}" />
        public IEnumerator<KeyValuePair<T, bool>> GetEnumerator()
        {
            return collection.Select(item => new KeyValuePair<T, bool>(item, IsDirty(item)))
                             .GetEnumerator();
        }

        /// <inheritdoc cref="IEnumerable" />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public int Count => collection.Count();

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool ContainsKey(T key)
        {
            return Keys.Contains(key);
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool TryGetValue(T key, out bool value)
        {
            value = IsDirty(key);
            return ContainsKey(key);
        }

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public bool this[T key] => IsDirty(key);

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public IEnumerable<T> Keys => collection;

        /// <inheritdoc cref="IDictionary{TKey,TValue}" />
        public IEnumerable<bool> Values => Keys.Select(IsDirty);

        /// <summary>
        /// Handler for an <see cref="INotifyCollectionChanged" />, where additions,
        /// removals, replacements and
        /// resets all result in the dirty state being updated.
        /// </summary>
        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                        foreach (var key in e.OldItems)
                            ClearDirty((T) key);
                    if (e.NewItems != null)
                        foreach (var key in e.NewItems)
                            MarkDirty((T) key);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearAllDirty();
                    foreach (var item in collection)
                        MarkDirty(item);
                    break;
            }
        }

        /// <summary>
        /// Has <paramref name="item" /> been altered since the last time its dirty state
        /// was reset.
        /// </summary>
        public bool IsDirty(T item)
        {
            return dirtyItems.Contains(item);
        }

        /// <summary>
        /// Marks <paramref name="item" /> as being dirty.
        /// </summary>
        public void MarkDirty(T item)
        {
            dirtyItems.Add(item);
        }

        /// <summary>
        /// Set the dirty state of <paramref name="item" /> to be <paramref name="dirty" />
        /// .
        /// </summary>
        public void SetDirty(T item, bool dirty)
        {
            if (dirty)
                MarkDirty(item);
            else
                ClearDirty(item);
        }

        /// <summary>
        /// Reset the dirty state of <paramref name="item" />.
        /// </summary>
        public void ClearDirty(T item)
        {
            dirtyItems.Remove(item);
        }

        /// <summary>
        /// Reset the dirty state of all items.
        /// </summary>
        public void ClearAllDirty()
        {
            dirtyItems.Clear();
        }
    }
}