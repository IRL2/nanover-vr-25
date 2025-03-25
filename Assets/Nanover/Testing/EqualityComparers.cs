using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace Nanover.Testing
{
    /// <summary>
    /// Compares if two matrixes are equal by comparing each row using unity's
    /// standard Vector4EqualityComparer.
    /// </summary>
    public class Matrix4x4RowEqualityComparer : IEqualityComparer<Matrix4x4>
    {
        private static Matrix4x4RowEqualityComparer _instance;

        public static Matrix4x4RowEqualityComparer Instance
        {
            get
            {
                _instance = _instance ?? new Matrix4x4RowEqualityComparer();
                return _instance;
            }
        }

        private readonly Vector4EqualityComparer vector4Comparer;

        public Matrix4x4RowEqualityComparer(float allowedError = 0.0001f)
        {
            vector4Comparer = new Vector4EqualityComparer(allowedError);
        }

        public bool Equals(Matrix4x4 x, Matrix4x4 y)
        {
            var equals = true;

            for (var row = 0; row < 4; ++row)
            {
                equals &= vector4Comparer.Equals(x.GetRow(row), y.GetRow(row));
            }

            return equals;
        }

        public int GetHashCode(Matrix4x4 matrix) => matrix.GetHashCode();
    }

    /// <summary>
    /// Compares if two TRS matrixes are equal based on the error in their
    /// independent translation, rotation, scale components.
    /// </summary>
    public class Matrix4x4TRSEqualityComparer : IEqualityComparer<Matrix4x4>
    {
        private static Matrix4x4TRSEqualityComparer _instance;

        public static Matrix4x4TRSEqualityComparer Instance
        {
            get
            {
                _instance = _instance ?? new Matrix4x4TRSEqualityComparer();
                return _instance;
            }
        }

        private readonly IEqualityComparer<Vector3> translationComparer;
        private readonly IEqualityComparer<Quaternion> rotationComparer;
        private readonly IEqualityComparer<Vector3> scaleComparer;

        public Matrix4x4TRSEqualityComparer(float allowedTranslationError = 0.0001f,
                                            float allowedRotationError = 0.0001f,
                                            float allowedScaleError = 0.0001f)
        {
            translationComparer = new Vector3EqualityComparer(allowedTranslationError);
            rotationComparer = new QuaternionEqualityComparer(allowedRotationError);
            scaleComparer = new Vector3EqualityComparer(allowedScaleError);
        }

        public bool Equals(Matrix4x4 x, Matrix4x4 y)
        {
            Assert.True(x.ValidTRS());
            Assert.True(y.ValidTRS());

            return translationComparer.Equals(x.GetColumn(3), y.GetColumn(3))
                && rotationComparer.Equals(x.rotation, y.rotation)
                && scaleComparer.Equals(x.lossyScale, y.lossyScale);
        }

        public int GetHashCode(Matrix4x4 matrix)
        {
            return matrix.GetHashCode();
        }
    }
}