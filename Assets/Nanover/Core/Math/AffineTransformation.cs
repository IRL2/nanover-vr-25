using System;
using UnityEngine;

namespace Nanover.Core.Math
{
    /// <summary>
    /// An transformation between two 3D spaces, defined by where it maps the
    /// three axes of cartesian space and an offset. This can represent any combination
    /// of rotations,
    /// reflections, translations, scaling and shears.
    /// </summary>
    /// <remarks>
    /// Every affine transformation can be represented as an augmented 4x4 matrix, with
    /// the three axes (with the 4th component 0) and the origin (with the 4th
    /// component 1) representing the three columns of the matrix.
    /// </remarks>
    [Serializable]
    public struct AffineTransformation : ITransformation
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

        /// <summary>
        /// The translation that this transformation applies.
        /// </summary>
        public Vector3 origin;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an affine transformation which maps the x, y and z directions to new
        /// vectors, and translates to a new origin.
        /// </summary>
        public AffineTransformation(Vector3 xAxis,
                                    Vector3 yAxis,
                                    Vector3 zAxis,
                                    Vector3 position)
        {
            this.xAxis = xAxis;
            this.yAxis = yAxis;
            this.zAxis = zAxis;
            this.origin = position;
        }
        
        /// <summary>
        /// Create an affine transformation from the upper 3x4 matrix.
        /// </summary>
        public AffineTransformation(Matrix4x4 matrix)
        {
            xAxis = matrix.GetColumn(0);
            yAxis = matrix.GetColumn(1);
            zAxis = matrix.GetColumn(2);
            origin = matrix.GetColumn(3);
        }

        #endregion


        #region Constants

        /// <summary>
        /// The identity transformation.
        /// </summary>
        public static AffineTransformation identity => new AffineTransformation(Vector3.right,
                                                                                Vector3.up,
                                                                                Vector3.forward,
                                                                                Vector3.zero);

        /// <summary>
        /// The magnitudes of the three axes that define this linear transformation.
        /// </summary>
        public Vector3 axesMagnitudes => new Vector3(xAxis.magnitude,
                                                     yAxis.magnitude,
                                                     zAxis.magnitude);
        
        #endregion


        #region Inverse

        /// <inheritdoc cref="ITransformation.inverse"/>
        public AffineTransformation inverse => new AffineTransformation(matrix.inverse);

        #endregion


        #region Matrices

        /// <inheritdoc cref="ITransformation.matrix"/>
        public Matrix4x4 matrix => 
            new Matrix4x4(xAxis, yAxis, zAxis, new Vector4(origin.x, origin.y, origin.z, 1));

        /// <inheritdoc cref="ITransformation.inverseMatrix"/>
        public Matrix4x4 inverseMatrix => inverse.matrix;

        #endregion


        #region Conversions

        public static implicit operator Matrix4x4(AffineTransformation transformation)
        {
            return transformation.matrix;
        }

        #endregion


        #region Multiplication

        public static AffineTransformation operator *(AffineTransformation a,
                                                      AffineTransformation b)
        {
            return new AffineTransformation(a.matrix * b.matrix);
        }

        #endregion


        #region Transformation of Points

        /// <inheritdoc cref="ITransformation.TransformPoint"/>
        public Vector3 TransformPoint(Vector3 point)
        {
            return point.x * xAxis
                 + point.y * yAxis
                 + point.z * zAxis
                 + origin;
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