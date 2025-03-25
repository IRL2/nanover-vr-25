using UnityEngine;

namespace Nanover.Core.Math
{
    /// <summary>
    /// Extension methods for Unity's <see cref="Matrix4x4" />
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Extract the translation component of the given TRS matrix. This is
        /// the worldspace origin of matrix's coordinate space.
        /// </summary>
        public static Vector3 GetTranslation(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        /// <summary>
        /// Extract the rotation component of the given TRS matrix. This is
        /// the quaternion that rotates worldspace forward, up, right vectors
        /// into the matrix's coordinate space.
        /// </summary>
        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
            return matrix.rotation;
        }

        /// <summary>
        /// Extract the scale component of the given TRS matrix, assuming it is orthogonal.
        /// </summary>
        public static Vector3 GetScale(this Matrix4x4 matrix)
        {
            return matrix.lossyScale;
        }

        /// <summary>
        /// Get the matrix that transforms from this matrix to another.
        /// </summary>
        /// <remarks>
        /// In Unity transformations are from local-space to world-space, so
        /// the transformation is multiplied on the right-hand side.
        /// </remarks>
        public static Matrix4x4 GetTransformationTo(this Matrix4x4 from, Matrix4x4 to)
        {
            return from.inverse * to;
        }

        /// <summary>
        /// Return this matrix transformed by the given transformation matrix.
        /// </summary>
        /// <remarks>
        /// In Unity transformations are from local-space to world-space, so
        /// the transformation is multiplied on the right-hand side.
        /// </remarks>
        public static Matrix4x4 TransformedBy(this Matrix4x4 matrix, Matrix4x4 transformation)
        {
            return matrix * transformation;
        }

        /// <inheritdoc cref="ITransformation.TransformPoint"/>
        public static Vector3 TransformPoint(this Matrix4x4 matrix, Vector3 point)
        {
            return matrix.MultiplyPoint3x4(point);
        }

        /// <inheritdoc cref="ITransformation.InverseTransformPoint"/>
        public static Vector3 InverseTransformPoint(this Matrix4x4 matrix, Vector3 point)
        {
            return matrix.inverse.MultiplyPoint3x4(point);
        }

        /// <inheritdoc cref="ITransformation.TransformDirection"/>
        public static Vector3 TransformDirection(this Matrix4x4 matrix, Vector3 point)
        {
            return matrix.MultiplyVector(point);
        }

        /// <inheritdoc cref="ITransformation.InverseTransformDirection"/>
        public static Vector3 InverseTransformDirection(this Matrix4x4 matrix, Vector3 point)
        {
            return matrix.inverse.MultiplyVector(point);
        }
    }
}