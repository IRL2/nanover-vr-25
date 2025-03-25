using JetBrains.Annotations;
using Nanover.Core.Science;
using UnityEngine;

namespace Nanover.Frame
{
    /// <summary>
    /// A particle which exists only as an index, pointing to various arrays which
    /// contain information such as name and type
    /// </summary>
    internal class ParticleReference : IParticle
    {
        private readonly int index;

        /// <summary>
        /// Create an ParticleReference that represents the particle at index in a frame
        /// </summary>
        public ParticleReference([NotNull] Frame frame, int index)
        {
            Frame = frame;
            this.index = index;
        }

        /// <summary>
        /// The topology the particle is a part of
        /// </summary>
        [NotNull]
        public Frame Frame { get; }

        /// <inheritdoc />
        public int Index => index;

        /// <inheritdoc />
        [CanBeNull]
        public string Type => Frame.ParticleTypes?[index];

        /// <inheritdoc />
        public Vector3 Position => Frame.ParticlePositions[index];

        public Element? Element => Frame.ParticleElements?[index];
    }
}