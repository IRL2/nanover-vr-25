using UnityEngine;

namespace Nanover.Core.Math
{
    /// <summary>
    /// Bundles position, rotation, and scale of a transformation.
    /// </summary>
    public struct Transformation : ITransformation
    {
        ITransformation ITransformation.inverse => new AffineTransformation(Matrix.inverse);

        Matrix4x4 ITransformation.matrix => Matrix;

        Matrix4x4 ITransformation.inverseMatrix => Matrix.inverse;

        Vector3 ITransformation.TransformPoint(Vector3 point) => Matrix.TransformPoint(point);

        Vector3 ITransformation.InverseTransformPoint(Vector3 point) =>
            Matrix.InverseTransformPoint(point);

        Vector3 ITransformation.TransformDirection(Vector3 point) =>
            Matrix.TransformDirection(point);

        Vector3 ITransformation.InverseTransformDirection(Vector3 point) =>
            Matrix.InverseTransformDirection(point);

        /// <summary>
        /// Construct a transformation from the translation, rotation, and
        /// scale of a Unity <see cref="Transform" /> relative to world space.
        /// </summary>
        /// <remarks>
        /// The scale is inherently lossy, as the composition of multiple
        /// transforms is not necessarily a transform.
        /// </remarks>
        public static Transformation FromTransformRelativeToWorld(Transform transform)
        {
            return new Transformation(transform.position,
                                      transform.rotation,
                                      transform.lossyScale);
        }

        /// <summary>
        /// Construct a transformation from the translation, rotation, and
        /// scale of a Unity <see cref="Transform" /> relative to world space.
        /// </summary>
        public static Transformation FromTransformRelativeToParent(Transform transform)
        {
            return new Transformation(transform.localPosition,
                                      transform.localRotation,
                                      transform.localScale);
        }

        /// <summary>
        /// The identity transformation.
        /// </summary>
        public static Transformation Identity =>
            new Transformation(Vector3.zero, Quaternion.identity, Vector3.one);

        /// <summary>
        /// Position of this transformation.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Rotation of this transformation.
        /// </summary>
        public Quaternion Rotation;

        /// <summary>
        /// Scale of this transformation.
        /// </summary>
        public Vector3 Scale;

        /// <summary>
        /// <see cref="Matrix4x4" /> representation of this transformation.
        /// </summary>
        public Matrix4x4 Matrix => Matrix4x4.TRS(Position, Rotation, Scale);

        /// <summary>
        /// The <see cref="Matrix4x4" /> representation of the inverse of this
        /// transformation. Note that the inverse itself cannot be necessarily represented
        /// as a single <see cref="Transformation" />.
        /// </summary>
        public Matrix4x4 InverseMatrix => Matrix.inverse;

        /// <summary>
        /// The inverse transpose of the matrix representation of this transformation. This
        /// is equivalent to the transformation with the same position and rotation, but
        /// with inverted scales.
        /// </summary>
        /// <remarks>
        /// Some shaders require the inverse transpose, as normal vectors transform using
        /// the inverse transpose of the transformation the positions undergo.
        /// </remarks>
        public Matrix4x4 InverseTransposeMatrix => Matrix4x4.TRS(Position,
                                                                 Rotation,
                                                                 new Vector3(1f / Scale.x,
                                                                             1f / Scale.y,
                                                                             1f / Scale.z));

        public Transformation(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// Set the transform's position, rotation and scale relative to its parent from
        /// this transformation.
        /// </summary>
        public void CopyToTransformRelativeToParent(Transform transform)
        {
            transform.localPosition = Position;
            transform.localRotation = Rotation;
            transform.localScale = Scale;
        }

        /// <summary>
        /// Set the transform's position, rotation and scale relative to the world space
        /// from this transformation.
        /// </summary>
        public void CopyToTransformRelativeToWorld(Transform transform)
        {
            // we are not allowed to set global scale directly in Unity, so
            // instead we unparent the object, make local changes, then reparent
            var parent = transform.parent;

            transform.parent = null;

            transform.localPosition = Position;
            transform.localRotation = Rotation;
            transform.localScale = Scale;

            transform.parent = parent;
        }

        public override string ToString()
        {
            var pos = Position;
            var rot = Rotation.eulerAngles;
            var scale = Scale;
            return
                $"Transformation(Position: ({pos.x}, {pos.y}, {pos.z}), Rotation: ({rot.x}, {rot.y}, {rot.z}), Scale: ({scale.x}, {scale.y}, {scale.z}))";
        }

        /// <summary>
        /// Convert to a transformation with unit scale, discarding any scaling associated
        /// with this transformation.
        /// </summary>
        public UnitScaleTransformation AsUnitTransformWithoutScale()
        {
            return new UnitScaleTransformation(Position, Rotation);
        }
    }
}