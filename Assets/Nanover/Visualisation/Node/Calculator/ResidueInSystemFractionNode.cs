using System;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Calculator
{
    /// <summary>
    /// Calculates the relative (0-1) fraction of each residue in the system.
    /// </summary>
    [Serializable]
    public class ResidueInSystemFractionNode : GenericFractionNode
    {
        [SerializeField]
        private IntArrayProperty particleResidues;

        [SerializeField]
        private IntProperty residueCount;

        protected override bool IsInputValid => particleResidues.HasNonEmptyValue()
                                             && residueCount.HasNonNullValue();

        protected override bool IsInputDirty => particleResidues.IsDirty
                                             || residueCount.IsDirty;

        protected override void ClearDirty()
        {
            particleResidues.IsDirty = false;
            residueCount.IsDirty = false;
        }

        protected override void GenerateArray(ref float[] array)
        {
            var particleResidues = this.particleResidues.Value;
            var residueCount = this.residueCount.Value;
            Array.Resize(ref array, particleResidues.Length);
            for (var i = 0; i < particleResidues.Length; i++)
                array[i] = particleResidues[i] / (residueCount - 1f);
        }
    }
}