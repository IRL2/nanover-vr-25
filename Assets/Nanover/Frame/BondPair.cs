using System;

namespace Nanover.Frame
{
    /// <summary>
    /// Represents a bond between two particles with indices A and B
    /// </summary>
    [Serializable]
    public struct BondPair
    {
        /// <summary>
        /// The index of the first particle
        /// </summary>
        public int A;

        /// <summary>
        /// The index of the second particle
        /// </summary>
        public int B;

        /// <summary>
        /// Construct a BondPair for a pair of two indices
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public BondPair(int a, int b)
        {
            A = a;
            B = b;
        }

        /// <summary>
        /// Get the first and second particles involved in this bond.
        /// </summary>
        public int this[int i]
        {
            get
            {
                if (i == 0)
                    return A;
                if (i == 1)
                    return B;
                throw new IndexOutOfRangeException();
            }
        }
    }
}