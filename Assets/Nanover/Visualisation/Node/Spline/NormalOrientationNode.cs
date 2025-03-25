using System;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Spline
{
    /// <summary>
    /// Rotates normals so within each sequence, the normals only rotate by up to 90 degrees.
    /// </summary>
    [Serializable]
    public class NormalOrientationNode : GenericOutputNode
    {
        /// <summary>
        /// The set of normals to rotate.
        /// </summary>
        [SerializeField]
        private Vector3ArrayProperty inputNormals = new Vector3ArrayProperty();

        /// <summary>
        /// The set of tangents, needed to calculate the binormals.
        /// </summary>
        [SerializeField]
        private Vector3ArrayProperty inputTangents = new Vector3ArrayProperty();

        /// <summary>
        /// A set of rotated normals that minimises the rotation of the normals.
        /// </summary>
        private Vector3ArrayProperty outputNormals = new Vector3ArrayProperty();

        /// <summary>
        /// Calculate rotated normals from an existing set of normals and tangents.
        /// </summary>
        public void RotateNormals(Vector3[] normals, Vector3[] tangents)
        {
            var outputNormals = this.outputNormals.Value;
            
            for (var j = 1; j < normals.Length; j++)
            {
                var prev = outputNormals[j - 1];
                var current = outputNormals[j];
                if (Vector3.Dot(prev, current) < 0)
                    outputNormals[j] = -current;
            }

            this.outputNormals.Value = outputNormals;
        }

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => inputNormals.HasNonNullValue()
                                             && inputTangents.HasNonNullValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => inputNormals.IsDirty
                                             || inputTangents.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            inputNormals.IsDirty = false;
            inputTangents.IsDirty = false;
        }

        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            outputNormals.Resize(inputNormals.Value.Length);
            Array.Copy(inputNormals, outputNormals, inputNormals.Value.Length);
            RotateNormals(outputNormals.Value, inputTangents.Value);
        }

        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            outputNormals.UndefineValue();
        }
    }
}