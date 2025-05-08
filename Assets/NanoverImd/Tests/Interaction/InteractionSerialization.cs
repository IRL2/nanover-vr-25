using System.Collections.Generic;
using NanoverImd.Interaction;
using NUnit.Framework;
using UnityEngine;

namespace NanoverImd.Tests.Interaction
{
    public class InteractionSerialization
    {
        [Test]
        public void SerializeInteraction()
        {
            var interaction = new ParticleInteraction()
            {
                Position = new Vector3(1f, 0.6f, -2.1f),
                Particles = new List<int>
                {
                    4,
                    7,
                    8,
                    11
                },
                MassWeighted = false,
                InteractionType = "spring"
            };

            var serialized = Nanover.Core.Serialization.Serialization.ToDataStructure(interaction);
            var deserialized =
                Nanover.Core.Serialization.Serialization.FromDataStructure<ParticleInteraction>(serialized);
            
            Assert.AreEqual(interaction.Position, deserialized.Position);
            Assert.AreEqual(interaction.Particles, deserialized.Particles);
            Assert.AreEqual(interaction.MassWeighted, deserialized.MassWeighted);
            Assert.AreEqual(interaction.InteractionType, deserialized.InteractionType);
        }
    }
}