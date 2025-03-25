using System;
using System.Collections.Generic;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Spline
{
    /// <summary>
    /// Get the indices of the end points of a set of sequences.
    /// </summary>
    [Serializable]
    public class SequenceEndPointsNode : GenericOutputNode
    {
        /// <summary>
        /// A set of sequences.
        /// </summary>
        [SerializeField]
        private SelectionArrayProperty sequences = new SelectionArrayProperty();

        /// <summary>
        /// The indices of the start and end of each sequence.
        /// </summary>
        private readonly IntArrayProperty filters = new IntArrayProperty();

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => sequences.HasValue;

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => sequences.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            sequences.IsDirty = false;
        }

        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            var list = new List<int>();
            foreach (var sequence in sequences.Value)
            {
                list.Add(sequence[0]);
                if (sequence.Count > 1)
                    list.Add(sequence[sequence.Count - 1]);
            }

            filters.Value = list.ToArray();
        }

        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            filters.UndefineValue();
        }
    }
}