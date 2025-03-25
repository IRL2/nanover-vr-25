using System;
using System.Linq;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Spline
{
    /// <summary>
    /// Generate a set of <see cref="ExtendedSplineSegment"/>s from a set of positions.
    /// </summary>
    /// <remarks>
    /// This is a sibling class to the standard <c>SplineNode</c> which extends the functionality
    /// to allow for each segment of a spline to have its own independent scale value.
    /// </remarks>
    [Serializable]
    public class ExtendedSplineNode : GenericOutputNode
    {
        [SerializeField]
        private IntArrayProperty sequenceCounts = new IntArrayProperty();

        [SerializeField]
        private Vector3ArrayProperty vertexPositions = new Vector3ArrayProperty();

        [SerializeField]
        private Vector3ArrayProperty vertexNormals = new Vector3ArrayProperty();

        [SerializeField]
        private Vector3ArrayProperty vertexTangents = new Vector3ArrayProperty();

        [SerializeField]
        private ColorArrayProperty vertexColors = new ColorArrayProperty();

        [SerializeField]
        private FloatArrayProperty vertexScales = new FloatArrayProperty();

        [SerializeField]
        private ColorProperty color;

        [SerializeField]
        private FloatProperty width;

        [SerializeField]
        private FloatArrayProperty residueRadii;

        private ExtendedSplineArrayProperty splineSegments = new ExtendedSplineArrayProperty();

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => sequenceCounts.HasNonNullValue()
                                             && vertexPositions.HasNonNullValue()
                                             && vertexNormals.HasNonNullValue()
                                             && vertexTangents.HasNonNullValue()
                                             && color.HasNonNullValue()
                                             && width.HasNonNullValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => sequenceCounts.IsDirty
                                             || vertexPositions.IsDirty
                                             || vertexNormals.IsDirty
                                             || vertexTangents.IsDirty
                                             || vertexColors.IsDirty
                                             || color.IsDirty
                                             || width.IsDirty
                                             || vertexScales.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            sequenceCounts.IsDirty = false;
            vertexPositions.IsDirty = false;
            vertexNormals.IsDirty = false;
            vertexTangents.IsDirty = false;
            vertexColors.IsDirty = false;
            color.IsDirty = false;
            width.IsDirty = false;
            vertexScales.IsDirty = false;
        }

        protected (Vector3 position, Vector3 tangent, Vector3 normal, UnityEngine.Color color, float
            size, float residueRadius) GetVertex(int offset)
        {
            var color = this.color.HasValue ? this.color.Value : UnityEngine.Color.white;
            var width = this.width.HasValue ? this.width.Value : 1f;
            
            return (vertexPositions.Value[offset],
                vertexTangents.Value[offset],
                vertexNormals.Value[offset],
                color * (vertexColors.HasValue
                    ? vertexColors.Value[offset]
                    : UnityEngine.Color.white),
                width * (vertexScales.HasValue ? vertexScales.Value[offset] : 1f),
                residueRadii.Value[offset]);
        }


        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            var segmentCount = sequenceCounts.Value.Sum(s => s - 1);

            this.splineSegments.Resize(segmentCount);

            var splineSegments = this.splineSegments.Value;
            
            var offset = 0;
            var segOffset = 0;

            foreach (var sequenceLength in sequenceCounts.Value)
            {
                if (sequenceLength == 0)
                    continue;

                var (startPosition, startTangent, startNormal, startColor, startWidth, startResidueRadius) =
                    GetVertex(offset);

                for (var i = 0; i < sequenceLength - 1; i++)
                {
                    var (endPosition, endTangent, endNormal, endColor, endWidth, endResidueRadius) =
                        GetVertex(offset + i + 1);

                    splineSegments[segOffset + i] = new ExtendedSplineSegment
                    {
                        StartPoint = startPosition,
                        StartNormal = startNormal,
                        StartTangent = startTangent,
                        StartColor = startColor,
                        StartScale = Vector3.one * startWidth,
                        EndPoint = endPosition,
                        EndNormal = endNormal,
                        EndTangent = endTangent,
                        EndColor = endColor,
                        EndScale = Vector3.one * endWidth,
                        StartRadius = startResidueRadius,
                        EndRadius = endResidueRadius
                    };

                    (startPosition, startTangent, startNormal, startColor, startWidth, startResidueRadius) =
                        (endPosition, endTangent, endNormal, endColor, endWidth, endResidueRadius);
                }

                offset += sequenceLength;
                segOffset += sequenceLength - 1;
            }

            this.splineSegments.Value = splineSegments;
        }

        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            splineSegments.UndefineValue();
        }
    }
}