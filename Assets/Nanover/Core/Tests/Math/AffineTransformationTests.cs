using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Math;
using Nanover.Testing;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Nanover.Core.Tests.Math
{
    public class AffineTransformationTests
    {
        private static readonly int TestCount = 128;

        public static IEnumerable<AffineTransformation> GetTransformations()
        {
            return SpatialTestData.GetRandomAffineTransformations(TestCount, 128947);
        }

        public static IEnumerable<(AffineTransformation, AffineTransformation)>
            GetTransformationPairs()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomAffineTransformations(TestCount, 214),
                SpatialTestData.GetRandomAffineTransformations(TestCount, 9214),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(AffineTransformation, Vector3)>
            GetTransformationAndVectors()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomAffineTransformations(TestCount, 532),
                SpatialTestData.GetRandomPositions(TestCount, 22),
                (a, b) => (a, b)
            );
        }


        #region Constants

        [Test]
        public void Identity()
        {
            Assert.AreEqual(Matrix4x4.identity, AffineTransformation.identity.matrix);
        }

        #endregion


        #region Inverse

        [Test]
        public void Inverse(
            [ValueSource(nameof(GetTransformations))] AffineTransformation transformation)
        {
            TransformationAssert.IsInverseCorrect(transformation);
        }

        #endregion


        #region Matrices

        [Test]
        public void InverseMatrix(
            [ValueSource(nameof(GetTransformations))] AffineTransformation transformation)
        {
            TransformationAssert.IsInverseMatrixCorrect(transformation);
        }

        #endregion


        #region Conversions

        [Test]
        public void AsMatrix(
            [ValueSource(nameof(GetTransformations))] AffineTransformation transformation)
        {
            MathAssert.AreEqual(transformation.matrix, (Matrix4x4) transformation);
        }

        #endregion


        #region Multiplication

        [Test]
        public void Multiplication(
            [ValueSource(nameof(GetTransformationPairs))]
            (AffineTransformation, AffineTransformation) input)
        {
            TransformationAssert.Multiplication(input, input.Item1 * input.Item2);
        }

        #endregion


        #region Transformation of Points

        [Test]
        public void TransformPoint(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (AffineTransformation, Vector3) input)
        {
            TransformationAssert.TransformPoint(input);
        }

        [Test]
        public void InverseTransformPoint(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (AffineTransformation, Vector3) input)
        {
            TransformationAssert.InverseTransformPoint(input);
        }

        #endregion


        #region Transformation of Directions

        [Test]
        public void TransformDirection(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (AffineTransformation, Vector3) input)
        {
            TransformationAssert.TransformDirection(input);
        }

        [Test]
        public void InverseTransformDirection(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (AffineTransformation, Vector3) input)
        {
            TransformationAssert.TransformDirection(input);
        }

        #endregion
    }
}