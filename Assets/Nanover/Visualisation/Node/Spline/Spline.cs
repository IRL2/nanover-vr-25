using System;
using System.Linq;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Spline
{
    /// <summary>
    /// Generate a set of <see cref="SplineSegment"/>s from a set of positions.
    /// </summary>
    [Serializable]
    public class SplineNode : GenericOutputNode
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
        private FloatProperty radius;

        private SplineArrayProperty splineSegments = new SplineArrayProperty();

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => sequenceCounts.HasNonNullValue()
                                             && vertexPositions.HasNonNullValue()
                                             && vertexNormals.HasNonNullValue()
                                             && vertexTangents.HasNonNullValue()
                                             && color.HasNonNullValue()
                                             && radius.HasNonNullValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => sequenceCounts.IsDirty
                                             || vertexPositions.IsDirty
                                             || vertexNormals.IsDirty
                                             || vertexTangents.IsDirty
                                             || vertexColors.IsDirty
                                             || color.IsDirty
                                             || radius.IsDirty
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
            radius.IsDirty = false;
            vertexScales.IsDirty = false;
        }

        protected (Vector3 position, Vector3 tangent, Vector3 normal, UnityEngine.Color color, float
            size) GetVertex(int offset)
        {
            var color = this.color.HasValue ? this.color.Value : UnityEngine.Color.white;
            var radius = this.radius.HasValue ? this.radius.Value : 1f;

            return (vertexPositions.Value[offset],
                    vertexTangents.Value[offset],
                    vertexNormals.Value[offset],
                    color * (vertexColors.HasValue
                                 ? vertexColors.Value[offset]
                                 : UnityEngine.Color.white),
                    radius * (vertexScales.HasValue ? vertexScales.Value[offset] : 1f));
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

                var (startPosition, startTangent, startNormal, startColor, startSize) =
                    GetVertex(offset);

                for (var i = 0; i < sequenceLength - 1; i++)
                {
                    var (endPosition, endTangent, endNormal, endColor, endSize) =
                        GetVertex(offset + i + 1);

                    splineSegments[segOffset + i] = new SplineSegment
                    {
                        StartPoint = startPosition,
                        StartNormal = startNormal,
                        StartTangent = startTangent,
                        StartColor = startColor,
                        StartScale = Vector3.one * startSize,
                        EndPoint = endPosition,
                        EndNormal = endNormal,
                        EndTangent = endTangent,
                        EndColor = endColor,
                        EndScale = Vector3.one * endSize
                    };

                    (startPosition, startTangent, startNormal, startColor, startSize) =
                        (endPosition, endTangent, endNormal, endColor, endSize);
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