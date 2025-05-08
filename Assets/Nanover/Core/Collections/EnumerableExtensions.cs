using System;
using System.Collections.Generic;
using System.Linq;

namespace Nanover.Core.Collections
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Enumerate over all pairs of adjacent items in a list, such that the set [A, B,
        /// C, D] yields the pairs (A, B), (B, C) and (C, D).
        /// </summary>
        public static IEnumerable<(TElement First, TElement Second)> GetPairs<TElement>(
            this IEnumerable<TElement> enumerable)
        {
            var started = false;
            var prev = default(TElement);
            foreach (var e in enumerable)
            {
                if (!started)
                {
                    started = true;
                    prev = e;
                }
                else
                {
                    yield return (prev, e);
                    prev = e;
                }
            }
        }

        /// <summary>
        /// Find the index of an item in an enumerable, returning -1 if the item is not
        /// present.
        /// </summary>
        public static int IndexOf<TElement>(this IEnumerable<TElement> list, TElement item)
        {
            var i = 0;
            foreach (var thing in list)
            {
                if (thing.Equals(item))
                    return i;
                i++;
            }

            return -1;
        }

        /// <summary>
        /// Yield an enumerable with the provided index skipped over.
        /// </summary>
        public static IEnumerable<T> WithoutIndex<T>(this IEnumerable<T> list, int index)
        {
            var i = 0;
            foreach (var item in list)
            {
                if (i != index)
                {
                    yield return item;
                }

                i++;
            }
        }

        /// <summary>
        /// Treat a single item as an <see cref="IEnumerable{T}" /> of one item.
        /// </summary>
        public static IEnumerable<T> AsEnumerable<T>(this T value)
        {
            yield return value;
        }

        /// <summary>
        /// Get all permutations of a set.
        /// </summary>
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> set)
        {
            var i = 0;
            foreach (var item in set)
            {
                var subsequence = set.WithoutIndex(i);
                foreach (var subpermutation in subsequence.GetPermutations())
                {
                    yield return item.AsEnumerable().Concat(subpermutation);
                }

                i++;
            }

            if (i == 1)
            {
                yield return set;
            }
        }

        /// <summary>
        /// Wraps an <see cref="IEnumerable{T}" /> so the <see cref="object.ToString" />
        /// method prints a list of the elements separated by commas.
        /// </summary>
        public static IEnumerable<T> AsPretty<T>(this IEnumerable<T> enumerable)
        {
            return new PrettyEnumerable<T>(enumerable);
        }

        /// <summary>
        /// Wraps an <see cref="IEnumerable{T}" /> so the <see cref="object.ToString" />
        /// method prints a list of the function <paramref name="toString" /> applied to
        /// each element, separated by commas.
        /// </summary>
        public static IEnumerable<T> AsPretty<T>(this IEnumerable<T> enumerable,
                                                 Func<T, string> toString)
        {
            return new PrettyEnumerable<T>(enumerable, toString);
        }
    }
}