using System;
using System.Linq;
using Nanover.Frame;
using Nanover.Visualisation.Properties;
using Nanover.Visualisation.Properties.Collections;
using Nanover.Visualisation.Property;
using UnityEngine;

namespace Nanover.Visualisation.Node.Spline
{
    [Serializable]
    public class TetrahedralSplineNode : GenericOutputNode
    {
        [SerializeField]
        private IntArrayProperty sequenceLengths = new IntArrayProperty();

        [SerializeField]
        private Vector3ArrayProperty positions = new Vector3ArrayProperty();

        [SerializeField]
        private Vector3ArrayProperty normals = new Vector3ArrayProperty();

        [SerializeField]
        private Vector3ArrayProperty tangents = new Vector3ArrayProperty();

        [SerializeField]
        private ColorArrayProperty colors = new ColorArrayProperty();

        [SerializeField]
        private FloatArrayProperty scales = new FloatArrayProperty();

        [SerializeField]
        private BondArrayProperty interiorBonds = new BondArrayProperty();

        [SerializeField]
        private ColorProperty color;

        [SerializeField]
        private FloatProperty radius;

        private Vector3ArrayProperty outputPositions = new Vector3ArrayProperty();

        private BondArrayProperty outputBonds = new BondArrayProperty();

        private SelectionArrayProperty outputFaces = new SelectionArrayProperty();

        private ColorArrayProperty outputColors = new ColorArrayProperty();

        private BondArrayProperty outputInteriorBonds = new BondArrayProperty();

        /// <inheritdoc cref="GenericOutputNode.IsInputValid"/>
        protected override bool IsInputValid => sequenceLengths.HasNonNullValue()
                                             && positions.HasNonNullValue()
                                             && normals.HasNonNullValue()
                                             && tangents.HasNonNullValue()
                                             && color.HasNonNullValue()
                                             && radius.HasNonNullValue();

        /// <inheritdoc cref="GenericOutputNode.IsInputDirty"/>
        protected override bool IsInputDirty => sequenceLengths.IsDirty
                                             || positions.IsDirty
                                             || normals.IsDirty
                                             || tangents.IsDirty
                                             || colors.IsDirty
                                             || color.IsDirty
                                             || radius.IsDirty
                                             || interiorBonds.IsDirty
                                             || scales.IsDirty;

        /// <inheritdoc cref="GenericOutputNode.ClearDirty"/>
        protected override void ClearDirty()
        {
            sequenceLengths.IsDirty = false;
            positions.IsDirty = false;
            normals.IsDirty = false;
            tangents.IsDirty = false;
            colors.IsDirty = false;
            color.IsDirty = false;
            radius.IsDirty = false;
            interiorBonds.IsDirty = false;
            scales.IsDirty = false;
        }

        /// <inheritdoc cref="GenericOutputNode.UpdateOutput"/>
        protected override void UpdateOutput()
        {
            if (sequenceLengths.IsDirty || !this.outputPositions.HasValue)
            {
                var pointCount = sequenceLengths.Value.Sum(i => i);
                var segmentCount = sequenceLengths.Value.Sum(i => i - 1);
                var bondcount = pointCount + 4 * segmentCount;

                this.outputPositions.Resize(pointCount * 2);
                this.outputBonds.Resize(bondcount);
                this.outputColors.Resize(pointCount * 2);
                this.outputFaces.Resize(segmentCount * 4);

                var outputBonds = this.outputBonds.Value;
                var outputFaces = this.outputFaces.Value;
                
                // Generate bonds between each pair of points representing a point of the spline.
                var bondIndex = 0;
                foreach (var sequenceLength in sequenceLengths)
                {
                    for (var i = 0; i < sequenceLength; i++)
                    {
                        outputBonds[bondIndex] = new BondPair(bondIndex * 2, bondIndex * 2 + 1);
                        bondIndex++;
                    }
                }

                var pointIndex = 0;
                var faceIndex = 0;
                foreach (var sequenceLength in sequenceLengths)
                {
                    for (var segment = 0; segment < sequenceLength - 1; segment++)
                    {
                        outputBonds[bondIndex] = new BondPair(pointIndex, pointIndex + 2);
                        outputBonds[bondIndex + 1] = new BondPair(pointIndex, pointIndex + 3);
                        outputBonds[bondIndex + 2] = new BondPair(pointIndex + 1, pointIndex + 2);
                        outputBonds[bondIndex + 3] = new BondPair(pointIndex + 1, pointIndex + 3);

                        outputFaces[faceIndex] = new[]
                            { pointIndex, pointIndex + 1, pointIndex + 2 };
                        outputFaces[faceIndex + 1] = new[]
                            { pointIndex, pointIndex + 1, pointIndex + 3 };
                        outputFaces[faceIndex + 2] = new[]
                            { pointIndex, pointIndex + 2, pointIndex + 3 };
                        outputFaces[faceIndex + 3] = new[]
                            { pointIndex + 1, pointIndex + 2, pointIndex + 3 };

                        bondIndex += 4;
                        faceIndex += 4;
                        pointIndex += 2;
                    }

                    pointIndex += 2;
                }

                this.outputBonds.Value = outputBonds;
                this.outputFaces.Value = outputFaces;

                interiorBonds.IsDirty = true;
            }

            if (interiorBonds.IsDirty)
            {
                if (interiorBonds.HasNonNullValue())
                {
                    var interiorBonds = this.interiorBonds.Value;

                    this.outputInteriorBonds.Resize(interiorBonds.Length);
                    var outputInteriorBonds = this.outputInteriorBonds.Value;
                    
                    for (var i = 0; i < interiorBonds.Length; i++)
                    {
                        outputInteriorBonds[i] = new BondPair(interiorBonds[i].A * 2,
                                                              interiorBonds[i].B * 2);
                    }

                    this.outputInteriorBonds.Value = outputInteriorBonds;
                }
                else
                {
                    outputInteriorBonds.UndefineValue();
                }
            }

            var outputPositions = this.outputPositions.Value;
            var outputColors = this.outputColors.Value;
            var positions = this.positions.Value;
            var tangents = this.tangents.Value;
            var normals = this.normals.Value;
            
            var index = 0;
            foreach (var sequenceLength in sequenceLengths)
            {
                for (var vertex = 0; vertex < sequenceLength; vertex++)
                {
                    var position = positions[index];
                    var tangent = tangents[index];
                    var normal = normals[index];
                    var binormal = Vector3.Cross(tangent, normal);
                    var color = this.color.Value * (colors.HasValue
                                                        ? colors.Value[index]
                                                        : UnityEngine.Color.white);
                    var radius = this.radius.Value * (scales.HasValue
                                                          ? scales.Value[index]
                                                          : 1f);

                    outputPositions[index * 2] = position + radius * binormal;
                    outputPositions[index * 2 + 1] = position - radius * binormal;
                    outputColors[index * 2] = color;
                    outputColors[index * 2 + 1] = color;
                    index++;
                }
            }

            this.outputPositions.Value = outputPositions;
            this.outputColors.Value = outputColors;
        }

        /// <inheritdoc cref="GenericOutputNode.ClearOutput"/>
        protected override void ClearOutput()
        {
            outputBonds.UndefineValue();
            outputColors.UndefineValue();
            outputFaces.UndefineValue();
            outputPositions.UndefineValue();
            outputInteriorBonds.UndefineValue();
        }
    }
}