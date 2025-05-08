using System;
using Nanover.Visualisation.Node.Protein;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    [Serializable]
    public class SecondaryStructureColor : VisualiserColorNode
    {
        [SerializeField]
        private SecondaryStructureArrayProperty residueSecondaryStructure;

        [SerializeField]
        private IntArrayProperty particleResidues;

        private UnityEngine.Color GetSecondaryStructureColor(SecondaryStructureAssignment i)
        {
            switch (i)
            {
                case SecondaryStructureAssignment.ThreeTenHelix:
                    return new UnityEngine.Color(0.6f, 0.0f, 0.5f);
                case SecondaryStructureAssignment.AlphaHelix:
                    return new UnityEngine.Color(1.0f, 0.0f, 0.5f);
                case SecondaryStructureAssignment.PiHelix:
                    return new UnityEngine.Color(0.4f, 0.0f, 0.5f);
                case SecondaryStructureAssignment.Turn:
                    return new UnityEngine.Color(0.4f, 0.5f, 1.0f);
                case SecondaryStructureAssignment.Sheet:
                    return new UnityEngine.Color(1.0f, 0.8f, 0.0f);
                default:
                    return UnityEngine.Color.white;
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
            colors.UndefineValue();
        }

        protected override void UpdateOutput()
        {
            var secondaryStructure = this.residueSecondaryStructure.Value;
            var residues = this.particleResidues.Value;
            var colorArray = colors.HasValue ? colors.Value : new UnityEngine.Color[0];
            Array.Resize(ref colorArray, residues.Length);
            for (var i = 0; i < residues.Length; i++)
                colorArray[i] = GetSecondaryStructureColor(secondaryStructure[residues[i]]);
            colors.Value = colorArray;
        }
    }
}