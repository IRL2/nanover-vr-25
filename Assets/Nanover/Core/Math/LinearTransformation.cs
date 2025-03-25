using System;
using UnityEngine;

namespace Nanover.Core.Math
{
    /// <summary>
    /// A linear transformation between two 3D spaces, defined by where it maps the
    /// three axes of cartesian space. This can represent any combination of rotations,
    /// reflections, scaling and shears.
    /// </summary>
    /// <remarks>
    /// All linear transformations preserve the origin.
    /// </remarks>
    [Serializable]
    public struct LinearTransformation : ITransformation
    {
        /// <inheritdoc cref="ITransformation.inverse"/>
        ITransformation ITransformation.inverse => inverse;

        #region Fields

        /// <summary>
        /// The vector to which this transformation maps the direction (1, 0, 0).
        /// </summary>
        public Vector3 xAxis;

        /// <summary>
        /// The vector to which this transformation maps the direction (0, 1, 0).
        /// </summary>
        public Vector3 yAxis;

        /// <summary>
        /// The vector to which this transformation maps the direction (0, 0, 1).
        /// </summary>
        public Vector3 zAxis;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a linear transformation which maps the x, y and z directions to new
        /// vectors.
        /// </summary>
        public LinearTransformation(Vector3 xAxis,
                                    Vector3 yAxis,
                                    Vector3 zAxis)
        {
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            this.zAxis = zAxis;
        }

        /// <summary>
        /// Create a linear transformation from the upper 3x3 matrix.
        /// </summary>
        public LinearTransformation(Matrix4x4 matrix)
        {
            xAxis = matrix.GetColumn(0);
            yAxis = matrix.GetColumn(1);
            zAxis = matrix.GetColumn(2);
        }

        #endregion


        #region Constants

        /// <summary>
        /// The identity transformation.
        /// </summary>
        public static LinearTransformation identity => new LinearTransformation(Vector3.right,
                                                                                Vector3.up,
                                                                                Vector3.forward);

        /// <summary>
        /// The magnitudes of the three axes that define this linear transformation.
        /// </summary>
        public Vector3 axesMagnitudes => new Vector3(xAxis.magnitude,
                                                     yAxis.magnitude,
                                                     zAxis.magnitude);

        #endregion


        #region Inverse

        /// <inheritdoc cref="ITransformation.inverse"/>
        public LinearTransformation inverse => new LinearTransformation(matrix.inverse);

        #endregion


        #region Matrices

        /// <inheritdoc cref="ITransformation.matrix"/>
        public Matrix4x4 matrix =>
            new Matrix4x4(xAxis, yAxis, zAxis, new Vector4(0, 0, 0, 1));

        /// <inheritdoc cref="ITransformation.inverseMatrix"/>
        public Matrix4x4 inverseMatrix => inverse.matrix;

        #endregion


        #region Conversions

        public static implicit operator Matrix4x4(LinearTransformation transformation)
        {
            return transformation.matrix;
        }

        public static implicit operator AffineTransformation(LinearTransformation transformation)
        {
            return new AffineTransformation(transformation.xAxis,
                                            transformation.yAxis,
                                            transformation.zAxis,
                                            Vector3.zero);
        }

        #endregion


        #region Multiplication

        public static LinearTransformation operator *(LinearTransformation a,
                                                      LinearTransformation b)
        {
            return new LinearTransformation(a.matrix * b.matrix);
        }

        #endregion


        #region Transformation of Points

        /// <inheritdoc cref="ITransformation.TransformPoint"/>
        public Vector3 TransformPoint(Vector3 point)
        {
            return point.x * xAxis
                 + point.y * yAxis
                 + point.z * zAxis;
        }

        /// <inheritdoc cref="ITransformation.InverseTransformPoint"/>
        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return inverse.TransformPoint(point);
        }

        #endregion


        #region Transformation of Directions

        /// <inheritdoc cref="ITransformation.TransformDirection"/>
        public Vector3 TransformDirection(Vector3 direction)
        {
            return direction.x * xAxis
                 + direction.y * yAxis
                 + direction.z * zAxis;
        }

        /// <inheritdoc cref="ITransformation.InverseTransformDirection"/>
        public Vector3 InverseTransformDirection(Vector3 direction)
        {
            return inverse.TransformDirection(direction);
        }

        #endregion
    }
}