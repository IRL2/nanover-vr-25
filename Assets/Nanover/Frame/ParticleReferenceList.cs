using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Nanover.Frame
{
    /// <summary>
    /// A virtual collection of particles, which allows the Struct of Arrays (SoA) data
    /// layout of Frame to be
    /// treated as an Array of Structs (AoS) data layout, for more intuitive access
    /// (Frame.Particles[i].Name
    /// instead of Frame.ParticlePositions[i])
    /// </summary>
    internal class ParticleReferenceList : IReadOnlyList<ParticleReference>
    {
        /// <summary>
        /// Creates an ParticleReferenceList for a Frame
        /// </summary>
        public ParticleReferenceList([NotNull] Frame frame)
        {
            Frame = frame;
        }

        /// <summary>
        /// Underlying frame
        /// </summary>
        [NotNull]
        public Frame Frame { get; }

        /// <inheritdoc />
        public IEnumerator<ParticleReference> GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        [NotNull]
        public ParticleReference this[int index] => new ParticleReference(Frame, index);

        /// <inheritdoc />
        public int Count => Frame.ParticlePositions?.Length ?? 0;
    }
}