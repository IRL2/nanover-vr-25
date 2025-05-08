using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Filter
{
    /// <summary>
    /// Filters particles by the name of their residues.
    /// </summary>
    [Serializable]
    public class ResidueNameFilterNode : VisualiserFilterNode
    {
        [SerializeField]
        private StringProperty pattern = new StringProperty();

        [SerializeField]
        private IntArrayProperty particleResidues = new IntArrayProperty();

        [SerializeField]
        private StringArrayProperty residueNames = new StringArrayProperty();

        /// <summary>
        /// Regex pattern to identify which residue names to filter.
        /// </summary>
        public IProperty<string> Pattern => pattern;

        /// <summary>
        /// The indices of the residue for each particle.
        /// </summary>
        public IProperty<int[]> ParticleResidues => particleResidues;

        /// <summary>
        /// The residue names.
        /// </summary>
        public IProperty<string[]> ResidueNames => residueNames;

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => pattern.HasNonNullValue()
                                             && particleResidues.HasNonEmptyValue()
                                             && residueNames.HasNonEmptyValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => pattern.IsDirty
                                             || particleResidues.IsDirty
                                             || residueNames.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            pattern.IsDirty = false;
            particleResidues.IsDirty = false;
            residueNames.IsDirty = false;
        }

        /// <inheritdoc cref="VisualiserFilterNode.MaximumFilterCount"/>
        protected override int MaximumFilterCount => particleResidues.Value.Length;

        /// <inheritdoc cref="VisualiserFilterNode.GetFilteredIndices"/>
        protected override IEnumerable<int> GetFilteredIndices()
        {
            var regex = new Regex(pattern.Value);
            for (var i = 0; i < particleResidues.Value.Length; i++)
            {
                if (regex.IsMatch(residueNames.Value[particleResidues.Value[i]]))
                    yield return i;
            }
        }
    }
}