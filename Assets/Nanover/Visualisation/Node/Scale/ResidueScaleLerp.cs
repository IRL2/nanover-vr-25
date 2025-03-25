using System;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;
using Nanover.Visualisation.Properties;

namespace Nanover.Visualisation.Node.Scale
{
    /// <summary>
    /// Represents a node that performs linear interpolation on normalised metric values associated
    /// with the residues of a given protein, providing linearly scaled output values.
    /// </summary>
    [Serializable]
    public class ResidueScaleLerp : GenericOutputNode
    {
        /// <summary>
        /// Start interpolation value, returned when <c>x</c> is zero or less.
        /// </summary>
        /// <remarks>
        /// This value will default to a value of one if undefined.
        /// </remarks>
        [SerializeField]
        private FloatProperty from = new FloatProperty();

        /// <summary>
        /// End interpolation value, returned when <c>x</c> is one or more.
        /// </summary>
        /// <remarks>
        /// This value will default to a value of one if undefined.
        /// </remarks>
        [SerializeField]
        private FloatProperty to = new FloatProperty();

        /// <summary>
        /// An array of normalised metric values that are to be interpolated.
        /// </summary>
        /// <remarks>
        /// These values will be linearly interpolated from the value specified by the <c>from</c>
        /// field to that of the <c>to</c> field.
        /// </remarks>
        [SerializeField]
        private FloatArrayProperty normalisedMetric;

        /// <summary>
        /// Linearly scaled output value backing field.
        /// </summary>
        [SerializeField]
        private readonly FloatArrayProperty output = new FloatArrayProperty();

        /// <summary>
        /// Linearly scaled output values.
        /// </summary>
        public IReadOnlyProperty<float[]> Output => output;

        /// <summary>
        /// Residue index values.
        /// </summary>
        /// <remarks>
        /// This specifies the indices of the residues to which each normalised metric value is
        /// associated. Commonly this is just an array over the range [0, n], however this is
        /// included to help deal with cases where the order of the normalised metric values might
        /// not correspond to the order in which the residues appear in the structure. 
        /// </remarks>
        [SerializeField]
        private IntArrayProperty residueIndices;

        /// <summary>
        /// Returns a boolean indicating the validity of the inputs.
        /// </summary>
        protected override bool IsInputValid =>
            normalisedMetric.HasNonEmptyValue() &&
            residueIndices.HasNonNullValue() &&
            normalisedMetric.Value.Length == residueIndices.Value.Length;

        /// <summary>
        /// Returns a boolean indicating if the input fields have updated.
        /// </summary>
        protected override bool IsInputDirty =>
            normalisedMetric.IsDirty || residueIndices.IsDirty || from.IsDirty || to.IsDirty;

        /// <summary>
        /// Clear <c>IsDirtry</c> flag of the attached input fields. 
        /// </summary>
        protected override void ClearDirty()
        {
            residueIndices.IsDirty = false;
            normalisedMetric.IsDirty = false;
            from.IsDirty = false;
            to.IsDirty = false;
        }

        /// <summary>
        /// Purge output fields.
        /// </summary>
        protected override void ClearOutput() => output.UndefineValue();

        /// <summary>
        /// Update output fields.
        /// </summary>
        protected override void UpdateOutput()
        {
            // Retrieve the residue id and metric values.
            var residues = this.residueIndices.Value;
            var metric = normalisedMetric.Value;
            
            // Ensure that the output array is allocated.
            var array = output.HasValue ? output.Value : new float[metric.Length];

            // Verify that the output array is of anticipated length.
            if (array.Length != metric.Length) 
                Array.Resize(ref array, metric.Length);

            // Get the interpolation bounds.
            var a = from.HasValue ? from.Value : 1f;
            var b = to.HasValue ? to.Value : 1f;

            // Linearly interpolate the normalised metric values over the supplied bounds.
            for (int i = 0; i < metric.Length; i++)
                array[i] = Mathf.Lerp(a, b, metric[residues[i]]);

            // Assign the results to the output array.
            output.Value = array;
        }
    }
}