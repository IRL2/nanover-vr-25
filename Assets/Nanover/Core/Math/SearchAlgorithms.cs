using System.Collections.Generic;

namespace Nanover.Core.Math
{
    public class SearchAlgorithms
    {
        /// <summary>
        /// Binary search to find an integer in a set of ordered integers.
        /// </summary>
        /// <param name="value">The value that is being searched for.</param>
        /// <param name="set">A set of integers ordered low to high.</param>
        public static bool BinarySearch(int value, IReadOnlyList<int> set)
        {
            var leftIndex = 0;
            var rightIndex = set.Count - 1;
            while (leftIndex <= rightIndex)
            {
                var midpointIndex = (leftIndex + rightIndex) / 2;
                var valueAtMidpoint = set[midpointIndex];
                if (valueAtMidpoint < value)
                    leftIndex = midpointIndex + 1;
                else if (valueAtMidpoint > value)
                    rightIndex = midpointIndex - 1;
                else
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Binary search to find the index of an integer in a set of ordered integers.
        /// </summary>
        /// <param name="value">The value that is being searched for.</param>
        /// <param name="set">A set of integers ordered low to high.</param>
        /// <returns>
        /// The index of <paramref name="value" /> in <paramref name="set" />, or
        /// -1 if value is not present.
        /// </returns>
        public static int BinarySearchIndex(int value, IReadOnlyList<int> set)
        {
            var leftIndex = 0;
            var rightIndex = set.Count - 1;
            while (leftIndex <= rightIndex)
            {
                var midpointIndex = (leftIndex + rightIndex) / 2;
                var valueAtMidpoint = set[midpointIndex];
                if (valueAtMidpoint < value)
                    leftIndex = midpointIndex + 1;
                else if (valueAtMidpoint > value)
                    rightIndex = midpointIndex - 1;
                else
                    return midpointIndex;
            }

            return -1;
        }
    }
}