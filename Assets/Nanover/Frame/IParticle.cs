using Nanover.Core.Science;
using UnityEngine;

namespace Nanover.Frame
{
    /// <summary>
    /// A particle in a trajectory. Minimally, it has a 0-based index in the frame and
    /// a position.
    /// </summary>
    public interface IParticle
    {
        /// <summary>
        /// The 0-based index of the particle
        /// </summary>
        int Index { get; }

        /// <summary>
        /// A string describing the 'type' of the particle
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The position of the particle
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// The element of the particle, if it is an atom.
        /// </summary>
        Element? Element { get; }
    }
}