using UnityEngine;

namespace Nanover.Core.Math
{
    /// <summary>
    /// A transformation consisting of a scaling by a uniform factor, a rotation and
    /// then a translation.
    /// </summary>
    /// <remarks>
    /// <see cref="UniformScaleTransformation" />s are closed under composition
    /// (combining two of them yields a third). These transformations preserve angles.
    /// </remarks>
    public struct UniformScaleTransformation : ITransformation
    {
        /// <inheritdoc cref="ITransformation.inverse"/>
        ITransformation ITransformation.inverse => inverse;

        #region Fields

        /// <summary>
        /// The translation this transformation applies. When considered as a
        /// transformation from an object's local space to world space, describes the
        /// position of the object.
        /// </summary>
        public Vector3 position;

        /// <summary>
        /// The rotation this transformation applies. When considered as a transformation
        /// from an object's local space to world space, describes the rotation of the
        /// object.
        /// </summary>
        public Quaternion rotation;

        /// <summary>
        /// The uniform scaling this transformation applies. When considered as a
        /// transformation from an object's local space to world space, describes the scale
        /// of the object.
        /// </summary>
        public float scale;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a transformation from its three actions.
        /// </summary>
        /// <param name="position">The translation this transformation applies.</param>
        /// <param name="rotation">The rotation this transformation applies.</param>
        /// <param name="scale">The scale this transformation applies.</param>
        public UniformScaleTransformation(Vector3 position, Quaternion rotation, float scale)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
        }

        #endregion


        #region Constants

        /// <summary>
        /// The identity transformation.
        /// </summary>
        public static UniformScaleTransformation identity =>
            new UniformScaleTransformation(Vector3.zero, Quaternion.identity, 1);

        #endregion


        #region Inverse

        /// <inheritdoc cref="ITransformation.inverse"/>
        public UniformScaleTransformation inverse
        {
            get
            {
                var inverseRotation = Quaternion.Inverse(rotation);
                return new UniformScaleTransformation(
                    inverseRotation * (-(1f / scale) * position),
                    inverseRotation,
                    1 / scale);
            }
        }

        #endregion


        #region Matrices

        /// <inheritdoc cref="ITransformation.matrix"/>
        public Matrix4x4 matrix => Matrix4x4.TRS(position, rotation, Vector3.one * scale);

        /// <inheritdoc cref="ITransformation.inverseMatrix"/>
        public Matrix4x4 inverseMatrix => inverse.matrix;

        #endregion


        #region Conversions

        public static implicit operator Matrix4x4(UniformScaleTransformation transformation)
        {
            return transformation.matrix;
        }

        public static implicit operator Transformation(UniformScaleTransformation transformation)
        {
            return new Transformation(transformation.position,
                                      transformation.rotation,
                                      transformation.scale * Vector3.one);
        }

        #endregion


        #region Multiplication

        public static UniformScaleTransformation operator *(UniformScaleTransformation a,
                                                            UniformScaleTransformation b)
        {
            return new UniformScaleTransformation(a.position + a.scale * (a.rotation * b.position),
                                                  a.rotation * b.rotation,
                                                  a.scale * b.scale);
        }

        public static Transformation operator *(UniformScaleTransformation a,
                                                Transformation b)
        {
            return new Transformation(a.position + a.scale * (a.rotation * b.Position),
                                      a.rotation * b.Rotation,
                                      a.scale * b.Scale);
        }

        #endregion


        #region Transformation of Points

        /// <inheritdoc cref="ITransformation.TransformPoint"/>
        public Vector3 TransformPoint(Vector3 pt)
        {
            return rotation * (scale * pt) + position;
        }

        /// <inheritdoc cref="ITransformation.InverseTransformPoint"/>
        public Vector3 InverseTransformPoint(Vector3 pt)
        {
            return inverse.TransformPoint(pt);
        }

        #endregion


        #region Transformation of Directions

        /// <inheritdoc cref="ITransformation.TransformDirection"/>
        public Vector3 TransformDirection(Vector3 direction)
        {
            return rotation * (scale * direction);
        }

        /// <inheritdoc cref="ITransformation.InverseTransformDirection"/>
        public Vector3 InverseTransformDirection(Vector3 pt)
        {
            return inverse.TransformDirection(pt);
        }

        #endregion


        #region TransformPose

        /// <summary>
        /// Get the transformation matrix that takes this transformation to the one provided.
        /// </summary>
        public Transformation TransformationTo(Transformation to)
        {
            var inverseRotation = Quaternion.Inverse(rotation);
            return new Transformation(inverseRotation * (to.Position - position) / scale,
                                      inverseRotation * to.Rotation,
                                      to.Scale / scale);
        }

        /// <summary>
        /// Right multiply by the provided transformation matrix to give another.
        /// </summary>
        public Transformation TransformBy(Transformation trans)
        {
            return this * trans;
        }

        public override string ToString()
        {
            var pos = position;
            var rot = rotation.eulerAngles;
            return
                $"UniformTransformation(Position: ({pos.x}, {pos.y}, {pos.z}), Rotation: ({rot.x}, {rot.y}, {rot.z}), Scale: ({scale}))";
        }

        #endregion
    }
}