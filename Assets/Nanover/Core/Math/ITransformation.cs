using UnityEngine;

namespace Nanover.Core.Math
{
    public interface ITransformation
    {
        /// <summary>
        /// The inverse of this transformation, which undoes this transformation.
        /// </summary>
        ITransformation inverse { get; }

        /// <summary>
        /// The 4x4 augmented matrix representing this transformation as it acts upon
        /// vectors and directions in homogeneous coordinates.
        /// </summary>
        Matrix4x4 matrix { get; }

        /// <summary>
        /// The 4x4 augmented matrix representing the inverse of this transformation as it
        /// acts upon vectors and directions in homogeneous coordinates.
        /// </summary>
        Matrix4x4 inverseMatrix { get; }

        #region Transformation of Points

        /// <summary>
        /// Transform a point in space using this transformation. When considered as a
        /// transformation from an object's local space to world space, this takes points
        /// in the object's local space to world space.
        /// </summary>
        Vector3 TransformPoint(Vector3 point);

        /// <summary>
        /// Transform a point in space using the inverse of this transformation. When
        /// considered as a transformation from an object's local space to world space,
        /// this takes points in world space to the object's local space.
        /// </summary>
        Vector3 InverseTransformPoint(Vector3 point);

        #endregion


        #region Transformation of Directions

        /// <summary>
        /// Transform a direction in space using this transformation. When considered as a
        /// transformation from an object's local space to world space, this takes
        /// directions in the object's local space to world space.
        /// </summary>
        Vector3 TransformDirection(Vector3 direction);

        /// <summary>
        /// Transform a direction in space using the inverse of this transformation. When
        /// considered as a transformation from an object's local space to world space,
        /// this takes directions in world space to the object's local space.
        /// </summary>
        Vector3 InverseTransformDirection(Vector3 direction);

        #endregion
    }
}