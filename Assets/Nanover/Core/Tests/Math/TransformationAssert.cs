using Nanover.Core.Math;
using UnityEngine;

namespace Nanover.Core.Tests.Math
{
    public static class TransformationAssert
    {
        #region Multiplication

        public static void Multiplication(
            (ITransformation, ITransformation) input,
            ITransformation result)
        {
            var (transformation1, transformation2) = input;
            var matrix1 = transformation1.matrix;
            var matrix2 = transformation2.matrix;
            MathAssert.AreEqual(matrix1 * matrix2, result.matrix);
        }

        #endregion


        #region Transformation of Points

        public static void TransformPoint((ITransformation, Vector3) input)
        {
            var (transformation, vector) = input;
            MathAssert.AreEqual(transformation.matrix.MultiplyPoint3x4(vector),
                                transformation.TransformPoint(vector));
        }

        public static void InverseTransformPoint((ITransformation, Vector3) input)
        {
            var (transformation, vector) = input;
            MathAssert.AreEqual(transformation.matrix.inverse.MultiplyPoint3x4(vector),
                                transformation.InverseTransformPoint(vector));
        }

        #endregion


        #region Transformation of Directions

        public static void TransformDirection((ITransformation, Vector3) input)
        {
            var (transformation, vector) = input;
            MathAssert.AreEqual(transformation.matrix.MultiplyVector(vector),
                                transformation.TransformDirection(vector));
        }

        public static void InverseTransformDirection((ITransformation, Vector3) input)
        {
            var (transformation, vector) = input;
            MathAssert.AreEqual(transformation.matrix.inverse.MultiplyVector(vector),
                                transformation.InverseTransformDirection(vector));
        }

        #endregion

        public static void IsInverseMatrixCorrect(ITransformation transformation)
        {
            var matrix = transformation.matrix;
            MathAssert.AreEqual(matrix.inverse, transformation.inverseMatrix);
        }

        public static void IsInverseCorrect(ITransformation transformation)
        {
            var matrix = transformation.matrix;
            MathAssert.AreEqual(matrix.inverse, transformation.inverse.matrix);
        }
    }
}