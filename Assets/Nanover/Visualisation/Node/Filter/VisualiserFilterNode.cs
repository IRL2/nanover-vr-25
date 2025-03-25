using System;
using System.Collections;
using System.Collections.Generic;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Property;

namespace Nanover.Visualisation.Node.Filter
{
    /// <summary>
    /// Filters particles by some criterea.
    /// </summary>
    public abstract class VisualiserFilterNode : GenericOutputNode
    {
        private readonly IntArrayProperty particleFilter = new IntArrayProperty();

        private int[] filter = new int[0];

        /// <summary>
        /// The maximum possible number of filtered indices. Used to allocate an initial array.
        /// </summary>
        protected abstract int MaximumFilterCount { get; }
        
        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            Array.Resize(ref filter, MaximumFilterCount);
            var i = 0;
            foreach (var f in GetFilteredIndices())
            {
                filter[i++] = f;
            }
            Array.Resize(ref filter, i);
            particleFilter.Value = filter;
        }
        
        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            Array.Resize(ref filter, 0);
            particleFilter.Value = filter;
        }
        
        /// <summary>
        /// Return the indices of particles which meet the criteria.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<int> GetFilteredIndices();

        /// <summary>
        /// List of particle indices parsed by this filter.
        /// </summary>
        public IReadOnlyProperty<int[]> ParticleFilter => particleFilter;
    }
}