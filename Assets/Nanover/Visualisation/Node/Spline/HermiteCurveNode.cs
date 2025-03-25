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
    /// Calculates normals and tangents for splines going through a set of points, assuming each segment is a hermite spline.
    /// </summary>
    [Serializable]
    public class HermiteCurveNode : GenericOutputNode
    {
        /// <summary>
        /// Positions to sample curve vertex positions.
        /// </summary>
        [SerializeField]
        private Vector3ArrayProperty positions = new Vector3ArrayProperty();

        public IProperty<Vector3[]> Positions => positions;

        /// <summary>
        /// Groups of indices of the Positions array which form sequences.
        /// </summary>
        [SerializeField]
        private IntArrayProperty sequences = new IntArrayProperty();

        /// <summary>
        /// Set of normals, for each vertex for each sequence.
        /// </summary>
        private Vector3ArrayProperty normals = new Vector3ArrayProperty();

        public IReadOnlyProperty<Vector3[]> Tangents => tangents;
        
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
        /// <param name="sequence">A sequence of indices for the position arrays.</param>
        /// <param name="positions">A set of positions.</param>
        /// <param name="offset">The offset of this sequence in the total vertex array.</param>
        /// <param name="normals">The array of normals to be partially filled for this sequence.</param>
        /// <param name="tangents">The array of tangents to be partially filled for this sequence.</param>
        /// <param name="shape">A factor to distort the tangents by.</param>
        private static void CalculateNormalsAndTangents(int sequenceLength,
                                                        int sequenceOffset,
                                                        IReadOnlyList<Vector3> positions,
                                                        ref Vector3[] normals,
                                                        ref Vector3[] tangents,
                                                        float shape)
        {
            var min = sequenceOffset;
            var max = sequenceOffset + sequenceLength;
            
            // Calculate tangents based offsets between positions.
            for (var j = min + 1; j < max - 1; j++)
                tangents[j] =
                    shape * (positions[j + 1] - positions[j - 1]);

            // Initial and final tangents are simply the tangents of the second and second from last tangents
            tangents[min] = tangents[min + 1];
            tangents[max - 1] = tangents[max - 2];

            // Compute normals by rejection of second derivative of curve from tangent
            for (var i = min; i < max - 1; i++)
            {
                var p1 = positions[i];
                var p2 = positions[i + 1];

                var m1 = tangents[i];
                var m2 = tangents[i + 1];

                var n0 = -6 * p1 - 4 * m1 + 6 * p2 - 2 * m2;
                var n1 = 6 * p1 + 2 * m1 - 6 * p2 + 4 * m2;

                n0 -= Vector3.Project(n0, m1);
                n1 -= Vector3.Project(n1, m2);

                normals[i] += n0;
                normals[i + 1] += n1;
            }

            // Normalize all normals
            for (var i = min; i < max; i++)
            {
                normals[i] = normals[i].normalized;
            }

            // Set the first normal
            normals[min] = (2 * normals[min + 1] - normals[min + 2]).normalized;
        }

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => sequences.HasNonNullValue()
                                             && positions.HasNonNullValue()
                                             && shape.HasNonNullValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => sequences.IsDirty
                                             || positions.IsDirty
                                             || shape.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            sequences.IsDirty = false;
            positions.IsDirty = false;
            shape.IsDirty = false;
        }

        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            if (sequences.IsDirty)
            {
                var vertexCount = sequences.Value.Sum();
                normals.Resize(vertexCount);
                tangents.Resize(vertexCount);
                positions.IsDirty = true;
            }

            if (positions.IsDirty || shape.IsDirty)
            {
                var offset = 0;
                var normals = this.normals.Value;
                var tangents = this.tangents.Value;
                foreach (var sequenceLength in sequences.Value)
                {
                    CalculateNormalsAndTangents(sequenceLength,
                                                offset,
                                                positions.Value,
                                                ref normals,
                                                ref tangents,
                                                shape);
                    offset += sequenceLength;
                }

                this.normals.Value = normals;
                this.tangents.Value = tangents;
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