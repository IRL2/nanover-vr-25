using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nanover.Visualisation.Node.Calculator
{
    /// <summary>
    /// List of indices representing a closed cycle, stored in canonical order (first
    /// index is the
    /// lowest, with the rest sorted such that the second index is lower than the last
    /// index.
    /// </summary>
    /// <remarks>
    /// For example, the indices (0, 1, 2, 3),  (0, 3, 2, 1) and (2, 3, 0, 1) are all
    /// converted
    /// to the canonical (0, 1, 2, 3).
    /// </remarks>
    [Serializable]
    public class Cycle : IReadOnlyList<int>
    {
        protected bool Equals(Cycle other)
        {
            if (other.Length != Length)
                return false;
            return Indices.SequenceEqual(other.Indices);
        }

        public IEnumerator<int> GetEnumerator()
        {
            foreach (var i in Indices)
                yield return i;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Cycle) obj);
        }

        public override int GetHashCode()
        {
            return Indices != null ? Indices.GetHashCode() : 0;
        }

        /// <summary>
        /// Indices stored in the cycle.
        /// </summary>
        [field: SerializeField]
        public int[] Indices { get; } = new int[0];

        public Cycle(params int[] indices)
        {
            Indices = ConvertToCanonicalIndices(indices);
        }

        private static int[] ConvertToCanonicalIndices(int[] indices)
        {
            if (indices.Length < 2)
                return indices;

            var min = int.MaxValue;
            var minIndex = int.MaxValue;
            for (var i = 0; i < indices.Length; i++)
            {
                var index = indices[i];
                if (index < min)
                {
                    min = index;
                    minIndex = i;
                }
            }

            // Ensure that the first index is the lowest
            if (minIndex != 0)
            {
                var newIndices = new int[indices.Length];

                for (var i = 0; i < indices.Length; i++)
                    newIndices[i] = indices[(i + minIndex) % indices.Length];

                indices = newIndices;
            }

            // Ensure the cycle is ordered so the second index is the next highest
            if (indices[1] > indices[indices.Length - 1])
            {
                var newIndices = new int[indices.Length];
                newIndices[0] = indices[0];
                for (var i = 1; i < indices.Length; i++)
                    newIndices[i] = indices[indices.Length - i];

                indices = newIndices;
            }

            return indices;
        }

        /// <summary>
        /// Length of the cycle.
        /// </summary>
        public int Count => Indices.Length;

        /// <summary>
        /// Length of the cycle.
        /// </summary>
        public int Length => Indices.Length;

        public int this[int i] => Indices[i];

        public override string ToString()
        {
            return $"Cycle({string.Join(", ", Indices)})";
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}