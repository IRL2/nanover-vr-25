using System;
using System.Collections.Generic;
using System.Linq;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Spline
{
    /// <summary>
    /// Calculates normals and tangents for splines going through a set of points, using the chemical structure for the normals.
    /// </summary>
    [Serializable]
    public class PolypeptideCurveNode : GenericOutputNode
    {
        /// <summary>
        /// Positions to sample curve vertex positions.
        /// </summary>
        [SerializeField]
        private Vector3ArrayProperty positions = new Vector3ArrayProperty();

        /// <summary>
        /// Groups of residue indices which form polypeptide curves
        /// </summary>
        [SerializeField]
        private SelectionArrayProperty residueSequences = new SelectionArrayProperty();

        [SerializeField]
        private StringArrayProperty atomNames = new StringArrayProperty();

        [SerializeField]
        private IntArrayProperty atomResidues = new IntArrayProperty();

        /// <summary>
        /// Set of normals, for each vertex for each sequence.
        /// </summary>
        private Vector3ArrayProperty normals = new Vector3ArrayProperty();

        /// <summary>
        /// Set of tangents, for each vertex for each sequence.
        /// </summary>
        private Vector3ArrayProperty tangents = new Vector3ArrayProperty();

        /// <summary>
        /// Factor which decides the weighting of the tangents.
        /// </summary>
        [SerializeField]
        private FloatProperty shape = new FloatProperty
        {
            Value = 1f
        };

        /// <summary>
        /// Calculate the normals and tangents for a certain sequence.
        /// </summary>
        private static void CalculateNormalsAndTangents(IReadOnlyList<int> sequence,
                                                        IReadOnlyList<Vector3> positions,
                                                        int offset,
                                                        ref Vector3[] normals,
                                                        ref Vector3[] tangents,
                                                        int[] carbonIndices,
                                                        int[] oxygenIndices,
                                                        int[] nitrogenIndices,
                                                        float shape)
        {
            var count = sequence.Count;

            // Calculate tangents based offsets between positions.
            for (var j = 1; j < count - 1; j++)
                tangents[offset + j] =
                    shape * (positions[carbonIndices[offset + j + 1]] -
                             positions[carbonIndices[offset + j - 1]]);

            // Initial and final tangents
            tangents[offset] = tangents[offset + 1];
            tangents[offset + count - 1] = tangents[offset + count - 2];

            // Compute normals from amino acid plane
            for (var i = 0; i < count; i++)
            {
                var carbonPosition = positions[carbonIndices[offset + i]];
                var nitrogenPosition = positions[nitrogenIndices[offset + i]];
                var oxygenPosition = positions[oxygenIndices[offset + i]];
                normals[offset + i] = Vector3.Cross(nitrogenPosition - carbonPosition,
                                                    oxygenPosition - carbonPosition)
                                             .normalized;
            }
        }

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => residueSequences.HasNonNullValue()
                                             && positions.HasNonNullValue()
                                             && shape.HasNonNullValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => residueSequences.IsDirty
                                             || positions.IsDirty
                                             || shape.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            residueSequences.IsDirty = false;
            positions.IsDirty = false;
            shape.IsDirty = false;
        }

        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            if (residueSequences.IsDirty)
            {
                var vertexCount = residueSequences.Value.Sum(a => a.Count);
                normals.Resize(vertexCount);
                tangents.Resize(vertexCount);
                positions.IsDirty = true;
                RecalculateResidues();
            }

            if (positions.IsDirty)
            {
                var offset = 0;
                var normals = this.normals.Value;
                var tangents = this.tangents.Value;
                foreach (var sequence in residueSequences.Value)
                {
                    CalculateNormalsAndTangents(sequence,
                                                positions.Value,
                                                offset,
                                                ref normals,
                                                ref tangents,
                                                alphaCarbonIndices,
                                                carbonylOxygenIndices,
                                                amineNitrogenIndices,
                                                shape);
                    offset += sequence.Count;
                }

                this.normals.Value = normals;
                this.tangents.Value = tangents;
            }
        }

        private int[] alphaCarbonIndices = new int[0];
        private int[] carbonylOxygenIndices = new int[0];
        private int[] amineNitrogenIndices = new int[0];

        private void RecalculateResidues()
        {
            var atomResidues = this.atomResidues.Value;
            var atomNames = this.atomNames.Value;
            
            var vertexCount = residueSequences.Value.Sum(a => a.Count);
            var residueIndices = residueSequences.Value.SelectMany(t => t).ToList();

            Array.Resize(ref alphaCarbonIndices, vertexCount);
            Array.Resize(ref carbonylOxygenIndices, vertexCount);
            Array.Resize(ref amineNitrogenIndices, vertexCount);

            for (var i = 0; i < atomResidues.Length; i++)
            {
                var resId = atomResidues[i];
                var index = residueIndices.IndexOf(resId);
                if (index == -1)
                    continue;
                var name = atomNames[i];
                if (name.Equals("ca", StringComparison.InvariantCultureIgnoreCase))
                    alphaCarbonIndices[index] = i;
                else if (name.Equals("o", StringComparison.InvariantCultureIgnoreCase))
                    carbonylOxygenIndices[index] = i;
                else if (name.Equals("n", StringComparison.InvariantCultureIgnoreCase))
                    amineNitrogenIndices[index] = i;
            }
        }

        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            normals.UndefineValue();
            tangents.UndefineValue();
        }
    }
}