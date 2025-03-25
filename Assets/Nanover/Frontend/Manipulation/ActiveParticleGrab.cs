using System;
using System.Collections.Generic;
using Nanover.Core.Math;
using UnityEngine;

namespace Nanover.Frontend.Manipulation
{
    /// <summary>
    /// Represents a grab between a particle and world-space position
    /// </summary>
    public class ActiveParticleGrab : IActiveManipulation
    {
        /// <inheritdoc cref="IActiveManipulation.ManipulationEnded"/>
        public event Action ManipulationEnded;

        /// <summary>
        /// A unique identifier for this interaction.
        /// </summary>
        public string Id { get; }
        
        /// <summary>
        /// The set of particle indices involved in this interaction.
        /// </summary>
        public IReadOnlyList<int> ParticleIndices => particleIndices;

        private readonly List<int> particleIndices = new List<int>();
        
        /// <summary>
        /// The position of the manipulator.
        /// </summary>
        public Vector3 GrabPosition { get; private set; }

        /// <summary>
        /// A set of properties associated with this manipulation.
        /// </summary>
        public bool ResetVelocities { get; set; } = false;

        /// <summary>
        /// Callback for when the grab position is updated.
        /// </summary>
        public event Action ParticleGrabUpdated;

        /// <summary>
        /// Optional ID of user owning this grab.
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Optional label for this grab.
        /// </summary>
        public string Label { get; set; }

        public ActiveParticleGrab(IEnumerable<int> particleIndices)
        {
            Id = Guid.NewGuid().ToString();
            this.particleIndices.AddRange(particleIndices);
        }

        /// <inheritdoc />
        public void UpdateManipulatorPose(UnitScaleTransformation manipulatorPose)
        {
            GrabPosition = manipulatorPose.position;
            ParticleGrabUpdated?.Invoke();
        }

        /// <inheritdoc />
        public void EndManipulation()
        {
            ManipulationEnded?.Invoke();
        }
    }
}