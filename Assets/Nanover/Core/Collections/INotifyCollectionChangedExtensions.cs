using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Nanover.Core.Collections
{
    public static class INotifyCollectionChangedExtensions
    {
        /// <summary>
        /// Expresses a <see cref="NotifyCollectionChangedEventArgs" /> as a set of changed
        /// values and a set of removed values.
        /// </summary>
        public static (IEnumerable<TValue> changes, IEnumerable<TValue> removals)
            AsChangesAndRemovals<TValue>(this NotifyCollectionChangedEventArgs args)
        {
            var newItems = args.NewItems?.Cast<TValue>().ToArray() ?? new TValue[0];
            var oldItems = args.OldItems?.Cast<TValue>().ToArray() ?? new TValue[0];
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return (newItems, new TValue[0]);
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    return (new TValue[0], oldItems);
                case NotifyCollectionChangedAction.Replace:
                    return (newItems, oldItems.Except(newItems));
                case NotifyCollectionChangedAction.Reset:
                    return (new TValue[0], oldItems);
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return (new TValue[0], new TValue[0]);
        }
    }
}