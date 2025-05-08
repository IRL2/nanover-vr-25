using System;
using System.Reflection;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;


namespace Nanover.Visualisation.Node.Color
{
    /// <summary>
    /// Colours protein residues using a heat-map according to some abstract metric.
    ///
    /// This will take the a float array output node which stores which stores a normalised metric
    /// value for each residue in the protein. These metric values are then used in conjunction with
    /// a colour gradient object to determine the colour of each residue.
    /// </summary>
    [Serializable]
    public class SecondaryStructureColorHeatmap: VisualiserColorNode
    {
        /// <summary>
        /// Array of normalised metric values for the residues.
        ///
        /// These abstract normalised metric values are used in conjunction with a heat-map to define
        /// the colour for each of the residues in the protein. There should be one, and only one,
        /// value for each and every residue. Such values should be normalised so that they roughly
        /// span the domain [0, 1]. The metric values within this array should be ordered so that
        /// they mach up with the order in which their associated carbon-alpha atoms appear in the
        /// base structure.
        /// </summary>
        [SerializeField]
        private FloatArrayProperty residueNormalisedMetricColour;

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
        /// Flattened array of RGBA colour values.
        /// </summary>
        /// <remarks>
        /// A flattened float array representing a collection of RGBA colour values. Each group
        /// of four consecutive floats corresponds to a single colour's red, green, blue, and
        /// alpha components.The array must have a length that is a multiple of 4. For example
        /// the, the following array would represent a gradient from red to blue:
        ///      R1  G1  B1  A1  R2  G2  B2  A2
        ///     {1f, 0f, 0f, 1f, 0f, 0f, 1f, 1f}
        /// </remarks>
        [SerializeField]
        private FloatArrayProperty gradientColourArray;


        /// <summary>
        /// The gradient used for the heat-map.
        /// </summary>
        /// <remarks>
        /// This will default to Viridis.
        /// </remarks>
        [SerializeField]
        private Gradient gradient = CreateDefaultGradient();


        /// <summary>
        /// Returns the default Viridis heat-map gradient.
        /// </summary>
        /// <returns>
        /// Viridis colour gradient.
        /// </returns>
        private static Gradient CreateDefaultGradient()
        {
            return new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(new UnityEngine.Color(0.27f, 0.00f, 0.33f, 1f), 0.00f),
                    new GradientColorKey(new UnityEngine.Color(0.23f, 0.32f, 0.55f, 1f), 0.25f),
                    new GradientColorKey(new UnityEngine.Color(0.13f, 0.57f, 0.55f, 1f), 0.50f),
                    new GradientColorKey(new UnityEngine.Color(0.37f, 0.79f, 0.38f, 1f), 0.75f),
                    new GradientColorKey(new UnityEngine.Color(0.99f, 0.91f, 0.15f, 1f), 1.00f)

                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1.0f, 0.0f),
                    new GradientAlphaKey(1.0f, 1.0f)
                }
            };
        }


        /// <summary>
        /// Converts a flattened array representing an Nx4 array of colours and alphas into a Unity Gradient object.
        /// The input array is assumed to be flattened row-wise, where each group of four consecutive values
        /// represents the red, green, blue, and alpha components of a colour.
        /// </summary>
        /// <param name="array">
        /// A flattened float array representing a collection of RGBA colour values.
        /// Each group of four consecutive floats corresponds to a single colour's red, green, blue, and alpha components.
        /// The array must have a length that is a multiple of 4.
        /// </param>
        /// <returns>
        /// A Unity Gradient object constructed from the input RGBA values.
        /// </returns>
        private Gradient CastArrayToGradient(float[] array)
        {

            // Constant defining the number of components (R, G, B, A) per colour. This is mostly
            // just here for the sake of visual clarity.
            const int componentsPerColour = 4;

            // Length of the array.
            int arrayLength = gradientColourArray.Value.Length;

            // Number of colours is calculated by dividing the array length by the number of components per colour (4)
            int numberOfColours = arrayLength / componentsPerColour;


            // Arrays for storing the Gradient's colour and alpha keys are initialised
            var colours = new GradientColorKey[numberOfColours];
            var alphas = new GradientAlphaKey[numberOfColours];

            // Step size is used to determine how the colours are spaced across the gradient. It
            // is assumed that i) the colour values are distributed evenly, and ii) alpha and
            // colour values have matching positions.
            float step = 1f / (numberOfColours - 1f);

            // A loop is used to iterate over the flattened array; each iteration processes one
            // colour (comprising 4 values: R, G, B, A).
            for (int i = 0; i < arrayLength; i += componentsPerColour)
            {
                // The index corresponding to the current colour is calculated by dividing
                // the loop variable by the number of components per colour.
                int index = i / componentsPerColour;

                // The "time" value is controls the the position of the colour on the gradient.
                float time = index * step;

                // A new GradientColorKey is created from the current colour's red, green, and blue
                // values and its associated "time" value.
                colours[index] = new GradientColorKey(new UnityEngine.Color(array[i], array[i + 1], array[i + 2]), time);

                // Repeat for the GradientAlphaKey instances.
                alphas[index] = new GradientAlphaKey(array[i + 3], time);

            }

            // Create a new gradient entity and set the colour and alpha values.
            var new_gradient = new Gradient();
            new_gradient.SetKeys(colours, alphas);

            // Finally, return the newly generated gradient
            return new_gradient;

        }

        /// <summary>
        /// Returns a boolean indicating the validity of the inputs.
        /// </summary>
        protected override bool IsInputValid =>
            residueNormalisedMetricColour.HasNonNullValue() &&
            residueIndices.HasNonNullValue() &&
            residueNormalisedMetricColour.Value.Length == residueIndices.Value.Length;

        // Note that no validity check is performed on the gradient array here as this class
        // can fallback to a default gradient if one is not supplied.

        /// <summary>
        /// Returns a boolean indicating if the input fields have updated.
        /// </summary>
        protected override bool IsInputDirty => 
            residueNormalisedMetricColour.IsDirty || 
            residueIndices.IsDirty || 
            gradientColourArray.IsDirty;

        /// <summary>
        /// Clear <c>IsDirtry</c> flag of the attached input fields. 
        /// </summary>
        protected override void ClearDirty()
        {
            residueNormalisedMetricColour.IsDirty = false;
            residueIndices.IsDirty = false;
            gradientColourArray.IsDirty = false;
        }

        /// <summary>
        /// Purge output fields.
        /// </summary>
        protected override void ClearOutput() => colors.UndefineValue();

        /// <summary>
        /// Update output fields.
        /// </summary>
        protected override void UpdateOutput()
        {
            // If a colour gradient array has been set or modified then update the gradient.
            if (gradientColourArray.HasNonEmptyValue() && gradientColourArray.IsDirty)
            {
                gradient = CastArrayToGradient(gradientColourArray.Value);
                // No need to unset the `HasValue` flag as that will be done by the entity that
                // calls this method.
            }

            // Retrieve the residue id and metric values.
            var residues = this.residueIndices.Value;
            var metrics = residueNormalisedMetricColour.Value;

            // Ensure that the output array is allocated.
            var colorArray = colors.HasValue ? colors.Value : Array.Empty<UnityEngine.Color>();

            // Verify that the output array is of anticipated length.
            if (colorArray.Length != metrics.Length)
                Array.Resize(ref colorArray, metrics.Length);

            // Linearly interpolate the normalised metric values over the colour gradient.
            for (var i = 0; i < metrics.Length; i++)
                colorArray[i] = gradient.Evaluate(metrics[residues[i]]);
            

            // Assign the results to the output array.
            colors.Value = colorArray;
        }
    }
}