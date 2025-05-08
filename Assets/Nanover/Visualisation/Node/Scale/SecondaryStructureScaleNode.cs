using System;
using Nanover.Visualisation.Node.Protein;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Scale
{
    /// <summary>
    /// Provides scales that depend on the secondary structure.
    /// </summary>
    [Serializable]
    public class SecondaryStructureScaleNode : VisualiserScaleNode
    {
        [SerializeField]
        private SecondaryStructureArrayProperty residueSecondaryStructure;

        [SerializeField]
        private IntArrayProperty particleResidues;

        [SerializeField]
        private float radius;

        [SerializeField]
        private float helixRadius;

        [SerializeField]
        private float turnRadius;

        [SerializeField]
        private float sheetRadius;

        private float GetSecondaryStructureScale(SecondaryStructureAssignment i)
        {
            switch (i)
            {
                case SecondaryStructureAssignment.ThreeTenHelix:
                    return helixRadius;
                case SecondaryStructureAssignment.AlphaHelix:
                    return helixRadius;
                case SecondaryStructureAssignment.PiHelix:
                    return helixRadius;
                case SecondaryStructureAssignment.Turn:
                    return turnRadius;
                case SecondaryStructureAssignment.Sheet:
                    return sheetRadius;
                default:
                    return radius;
            }
        }


        protected override bool IsInputValid => residueSecondaryStructure.HasNonEmptyValue()
                                             && particleResidues.HasNonNullValue();

        protected override bool IsInputDirty => residueSecondaryStructure.IsDirty
                                             || particleResidues.IsDirty;

        protected override void ClearDirty()
        {
            residueSecondaryStructure.IsDirty = false;
            particleResidues.IsDirty = false;
        }

        protected override void ClearOutput()
        {
            scales.UndefineValue();
        }

        protected override void UpdateOutput()
        {
            var secondaryStructure = this.residueSecondaryStructure.Value;
            var residues = this.particleResidues.Value;
            var scaleArray = scales.Resize(residues.Length);
            for (var i = 0; i < residues.Length; i++)
                scaleArray[i] = GetSecondaryStructureScale(secondaryStructure[residues[i]]);

            scales.Value = scaleArray;
        }
    }
}