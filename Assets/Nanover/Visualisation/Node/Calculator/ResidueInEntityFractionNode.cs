using System;
using System.Collections.Generic;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Calculator
{
    /// <summary>
    /// Calculates the relative (0-1) fraction of each residue in an entity.
    /// </summary>
    [Serializable]
    public class ResidueInEntityFractionNode : GenericFractionNode
    {
        [SerializeField]
        private IntArrayProperty particleResidues;

        [SerializeField]
        private IntArrayProperty residueEntities;

        [SerializeField]
        private IntProperty residueCount;

        [SerializeField]
        private IntProperty entityCount;

        protected override bool IsInputValid => particleResidues.HasNonEmptyValue()
                                             && residueCount.HasNonNullValue()
                                             && residueEntities.HasNonEmptyValue()
                                             && entityCount.HasNonNullValue();

        protected override bool IsInputDirty => particleResidues.IsDirty
                                             || residueCount.IsDirty
                                             || residueEntities.IsDirty
                                             || entityCount.IsDirty;

        protected override void ClearDirty()
        {
            particleResidues.IsDirty = false;
            residueCount.IsDirty = false;
            residueEntities.IsDirty = false;
            entityCount.IsDirty = false;
        }

        protected override void GenerateArray(ref float[] array)
        {
            var particleResidues = this.particleResidues.Value;
            var residueCount = this.residueCount.Value;
            var residueEntities = this.residueEntities.Value;
            
            var entitySize = new int[entityCount];
            for (var i = 0; i < entityCount; i++)
                entitySize[i] = 0;

            var residueRelativeIndex = new int[residueCount];

            for (var i = 0; i < residueCount; i++)
            {
                residueRelativeIndex[i] = entitySize[residueEntities[i]]++;
            }

            Array.Resize(ref array, particleResidues.Length);
            for (var i = 0; i < particleResidues.Length; i++)
            {
                var residue = particleResidues[i];
                array[i] = residueRelativeIndex[residue] / (entitySize[residueEntities[residue]] - 0.9999f);
            }
        }
    }
}