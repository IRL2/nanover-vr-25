using System;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Calculator
{
    /// <summary>
    /// Calculates the relative (0-1) fraction of each particle in the system.
    /// </summary>
    [Serializable]
    public class ParticleInSystemFractionNode : GenericFractionNode
    {
        [SerializeField]
        private IntProperty particleCount;

        protected override bool IsInputValid => particleCount.HasValue;
        
        protected override bool IsInputDirty => particleCount.IsDirty;

        protected override void ClearDirty()
        {
            particleCount.IsDirty = false;
        }

        protected override void GenerateArray(ref float[] array)
        {
            var particleCount = this.particleCount.Value;
            Array.Resize(ref array, particleCount);
            for (var i = 0; i < particleCount; i++)
                array[i] = i / (particleCount - 1f);
        }
    }
}