using System.Runtime.InteropServices;
using Nanover.Core.Math;
using Nanover.Frame;
using UnityEngine;

namespace Nanover.Visualisation.Utility
{
    /// <summary>
    /// Utility methods for setting up Nanover-specific instancing parameters
    /// for shaders.
    /// </summary>
    public static class InstancingUtility
    {
        /// <summary>
        /// Enable the position array in the shader and set the position values.
        /// </summary>
        public static void SetPositions(IndirectMeshDrawCommand command, Vector3[] positions)
        {
            command.SetKeyword("POSITION_ARRAY");
            command.SetDataBuffer("PositionArray", positions);
        }

        /// <summary>
        /// Enable the color array in the shader and set the color values.
        /// </summary>
        public static void SetColors(IndirectMeshDrawCommand command, Color[] colors)
        {
            command.SetKeyword("COLOR_ARRAY");
            command.SetDataBuffer("ColorArray", colors);
        }

        /// <summary>
        /// Enable the scales array in the shader and set the scale values.
        /// </summary>
        public static void SetScales(IndirectMeshDrawCommand command, float[] scales)
        {
            command.SetKeyword("SCALE_ARRAY");
            command.SetDataBuffer("ScaleArray", scales);
        }

        /// <summary>
        /// Enable the edge array in the shader and set the edge values.
        /// </summary>
        public static void SetEdges(IndirectMeshDrawCommand command, BondPair[] edges)
        {
            command.SetKeyword("EDGE_ARRAY");
            command.SetDataBuffer("EdgeArray", edges);
        }

        /// <summary>
        /// Enable the edge count array in the shader and set the edge count values.
        /// </summary>
        public static void SetEdgeCounts(IndirectMeshDrawCommand command, int[] edgeCounts)
        {
            command.SetKeyword("EDGE_COUNT_ARRAY");
            command.SetDataBuffer("EdgeCountArray", edgeCounts);
        }

        /// <summary>
        /// Copy the transform's world to object and object to world transform
        /// matrixes into the shader.
        /// </summary>
        public static void SetTransform(IndirectMeshDrawCommand command, Transform transform)
        {
            var trs = Transformation.FromTransformRelativeToWorld(transform);
            command.SetMatrix("WorldToObject", trs.InverseMatrix);
            command.SetMatrix("ObjectToWorld", trs.Matrix);
            command.SetMatrix("ObjectToWorldInverseTranspose", trs.InverseTransposeMatrix);
        }

        public static ComputeBuffer CreateBuffer<T>(T[] array)
        {
            var buffer = new ComputeBuffer(array.Length,
                                           Marshal.SizeOf<T>(),
                                           ComputeBufferType.Default);
            buffer.SetData(array);
            return buffer;
        }
    }
}