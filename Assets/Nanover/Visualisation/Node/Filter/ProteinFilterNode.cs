using System;
using System.Collections.Generic;
using Nanover.Core.Science;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Filter
{
    /// <summary>
    /// Filters particles by if they're in a standard amino acid, optionally excluding hydrogens.
    /// </summary>
    [Serializable]
    public class ProteinFilterNode : VisualiserFilterNode
    {
        [SerializeField]
        private BoolProperty includeHydrogens = new BoolProperty()
        {
            Value = true
        };

        [SerializeField]
        private BoolProperty includeWater = new BoolProperty()
        {
            Value = true
        };

        [SerializeField]
        private BoolProperty includeNonStandardResidues = new BoolProperty()
        {
            Value = true
        };

        [SerializeField]
        private IntArrayProperty particleResidues = new IntArrayProperty();

        [SerializeField]
        private ElementArrayProperty particleElements = new ElementArrayProperty();

        [SerializeField]
        private StringArrayProperty residueNames = new StringArrayProperty();

        /// <summary>
        /// Should hydrogens be included?
        /// </summary>
        public IProperty<bool> IncludeHydrogens => includeHydrogens;

        /// <summary>
        /// Should water be included?
        /// </summary>
        public IProperty<bool> IncludeWater => includeWater;

        /// <summary>
        /// Should non standard residues be included?
        /// </summary>
        public IProperty<bool> IncludeNonStandardResidues => includeNonStandardResidues;

        /// <summary>
        /// The indices of the residue for each particle.
        /// </summary>
        public IProperty<int[]> ParticleResidues => particleResidues;

        /// <summary>
        /// The residue names.
        /// </summary>
        public IProperty<string[]> ResidueNames => residueNames;

        /// <summary>
        /// The particle elements.
        /// </summary>
        public IProperty<Element[]> ParticleElements => particleElements;

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => particleResidues.HasNonEmptyValue()
                                             && residueNames.HasNonEmptyValue()
                                             && particleElements.HasNonEmptyValue()
                                             && includeHydrogens.HasNonNullValue()
                                             && includeWater.HasNonNullValue()
                                             && includeNonStandardResidues.HasNonNullValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => particleResidues.IsDirty
                                             || particleElements.IsDirty
                                             || residueNames.IsDirty
                                             || includeHydrogens.IsDirty
                                             || includeWater.IsDirty
                                             || includeNonStandardResidues.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            particleResidues.IsDirty = false;
            residueNames.IsDirty = false;
            includeHydrogens.IsDirty = false;
            particleElements.IsDirty = false;
            includeWater.IsDirty = false;
            includeNonStandardResidues.IsDirty = false;
        }

        /// <inheritdoc cref="VisualiserFilterNode.MaximumFilterCount"/>
        protected override int MaximumFilterCount => particleResidues.Value.Length;

        /// <inheritdoc cref="VisualiserFilterNode.GetFilteredIndices"/>
        protected override IEnumerable<int> GetFilteredIndices()
        {
            // Cache properties for performance
            var includeHydrogens = this.includeHydrogens.Value;
            var includeWater = this.includeWater.Value;
            var includeNonStandardResidues = this.includeNonStandardResidues.Value;
            var residueNames = this.residueNames.Value;
            var particleResidues = this.particleResidues.Value;
            var particleElements = this.particleElements.Value;
            
            var isResOkay = new bool[residueNames.Length];
            for (var j = 0; j < residueNames.Length; j++)
            {
                var resname = residueNames[j];
                if (resname == "HOH")
                    isResOkay[j] = includeWater;
                else
                    isResOkay[j] = includeNonStandardResidues
                                || AminoAcid.IsStandardAminoAcid(resname);
            }

            for (var i = 0; i < particleResidues.Length; i++)
            {
                if (!includeHydrogens && particleElements[i] == Element.Hydrogen)
                    continue;
                if(!isResOkay[particleResidues[i]])
                    continue;
                yield return i;
            }
        }
    }
}