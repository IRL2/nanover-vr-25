using System;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Color
{
    /// <summary>
    /// Visualiser node which generates colors based upon residue names.
    /// </summary>
    [Serializable]
    public class ResidueNameColorNode : VisualiserColorNode
    {
#pragma warning disable 0649
        [SerializeField]
        private StringArrayProperty residueNames = new StringArrayProperty();

        [SerializeField]
        private IntArrayProperty particleResidues = new IntArrayProperty();

        [SerializeField]
        private StringColorMapping mapping;
#pragma warning restore 0649

        protected override bool IsInputValid => residueNames.HasNonEmptyValue() 
                                             && particleResidues.HasNonEmptyValue();

        protected override bool IsInputDirty => residueNames.IsDirty
                                             || particleResidues.IsDirty;
        protected override void ClearDirty()
        {
            residueNames.IsDirty = false;
            particleResidues.IsDirty = false;
        }

        protected override void UpdateOutput()
        {
            var particleResidues = this.particleResidues.Value;
            var residueNames = this.residueNames.Value;
            var colorArray = colors.HasValue ? colors.Value : new UnityEngine.Color[0];
            Array.Resize(ref colorArray, particleResidues.Length);
            for (var i = 0; i < particleResidues.Length; i++)
                colorArray[i] = mapping.GetColor(residueNames[particleResidues[i]]);
            colors.Value = colorArray;
        }

        protected override void ClearOutput()
        {
            colors.UndefineValue();
        }
    }
}