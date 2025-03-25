using System;
using System.Collections.Generic;
using Nanover.Visualisation.Properties;
using UnityEngine;

namespace Nanover.Visualisation.Node.Protein
{
    /// <summary>
    /// Works out groupings of continuous entity IDs.
    /// </summary>
    [Serializable]
    public class ByEntitySequenceNode : GenericOutputNode
    {
        [SerializeField]
        private IntArrayProperty residueEntities = new IntArrayProperty();

        [SerializeField]
        private IntArrayProperty particleResidues = new IntArrayProperty();

        private IntArrayProperty entitySequenceLengths = new IntArrayProperty();

        protected override bool IsInputValid => residueEntities.HasValue
                                             && particleResidues.HasValue;

        protected override bool IsInputDirty => residueEntities.IsDirty
                                             || particleResidues.IsDirty;

        protected override void ClearDirty()
        {
            residueEntities.IsDirty = false;
            particleResidues.IsDirty = false;
        }

        protected override void UpdateOutput()
        {
            var particleCount = this.particleResidues.Value.Length;
            var particleResidues = this.particleResidues.Value;
            var residueEntities = this.residueEntities.Value;
            
            var l = new List<int>();
            var j = 0;
            var e = -1;
            for (var i = 0; i < particleCount; i++)
            {
                var residue = particleResidues[i];
                var entity = residueEntities[residue];
                if (e != entity)
                {
                    if (j > 0)
                        l.Add(j);
                    e = entity;
                    j = 1;
                }
                else
                {
                    j++;
                }
            }

            if (j > 0)
                l.Add(j);

            entitySequenceLengths.Value = l.ToArray();
        }

        protected override void ClearOutput()
        {
            entitySequenceLengths.UndefineValue();
        }
    }
}