using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

namespace NanoverImd.Interaction
{
    /// <summary>
    /// An interaction with an iMD enabled simulation.
    /// </summary>
    [DataContract]
    public class ParticleInteraction
    {
        /// <summary>
        /// The position of the interaction.
        /// </summary>
        [DataMember(Name = "position")]
        public Vector3 Position;

        /// <summary>
        /// The particle indices involved in the interaction.
        /// </summary>
        [DataMember(Name = "particles")]
        public List<int> Particles;

        /// <summary>
        /// The interaction type, such as 'string' or 'gaussian'.
        /// </summary>
        [DataMember(Name = "interaction_type")]
        public string InteractionType = "gaussian";

        /// <summary>
        /// The scale of the force.
        /// </summary>
        [DataMember(Name = "scale")]
        public float Scale = 1f;

        /// <summary>
        /// Should this interaction be mass weighted?
        /// </summary>
        [DataMember(Name = "mass_weighted")]
        public bool MassWeighted = true;

        /// <summary>
        /// Should this interaction reset velocities after being applied?
        /// </summary>
        [DataMember(Name = "reset_velocities")]
        public bool ResetVelocities = false;
            
        /// <summary>
        /// The maximum force asserted by this interaction.
        /// </summary>
        [DataMember(Name = "max_force")]
        public float MaxForce = float.PositiveInfinity;
            
        /// <summary>
        /// Arbitrary key-value data associated with this interaction.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object> Other = new Dictionary<string, object>();
    }
}