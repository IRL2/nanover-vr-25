using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Math;
using Nanover.Testing;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Nanover.Core.Tests.Math
{
    public class LinearTransformationTests
    {
        private static readonly int TestCount = 128;

        public static IEnumerable<LinearTransformation> GetTransformations()
        {
            return SpatialTestData.GetRandomLinearTransformations(TestCount, 2412);
        }

        public static IEnumerable<(LinearTransformation, LinearTransformation)>
            GetTransformationPairs()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomLinearTransformations(TestCount, 82),
                SpatialTestData.GetRandomLinearTransformations(TestCount, 940),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(LinearTransformation, Vector3)>
            GetTransformationAndVectors()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomLinearTransformations(TestCount, 21),
                SpatialTestData.GetRandomPositions(TestCount, 2918),
                (a, b) => (a, b)
            );
        }


        #region Constants

        [Test]
        public void Identity()
        {
            Assert.AreEqual(Matrix4x4.identity, LinearTransformation.identity.matrix);
        }

        #endregion


        #region Inverse

        [Test]
        public void Inverse(
            [ValueSource(nameof(GetTransformations))] LinearTransformation transformation)
        {
            TransformationAssert.IsInverseCorrect(transformation);
        }

        #endregion


        #region Matrices

        [Test]
        public void InverseMatrix(
            [ValueSource(nameof(GetTransformations))] LinearTransformation transformation)
        {
            TransformationAssert.IsInverseMatrixCorrect(transformation);
        }

        #endregion


        #region Conversions

        [Test]
        public void AsMatrix(
            [ValueSource(nameof(GetTransformations))] LinearTransformation transformation)
        {
            MathAssert.AreEqual(transformation.matrix, (Matrix4x4) transformation);
        }

        [Test]
        public void AsAffineTransformation(
            [ValueSource(nameof(GetTransformations))] LinearTransformation transformation)
        {
            MathAssert.AreEqual(transformation.matrix,
                                ((AffineTransformation) transformation).matrix);
        }

        #endregion


        #region Multiplication

        [Test]
        public void Multiplication(
            [ValueSource(nameof(GetTransformationPairs))]
            (LinearTransformation, LinearTransformation) input)
        {
            TransformationAssert.Multiplication(input, input.Item1 * input.Item2);
        }

        #endregion


        #region Transformation of Points

        [Test]
        public void TransformPoint(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (LinearTransformation, Vector3) input)
        {
            TransformationAssert.TransformPoint(input);
        }

        [Test]
        public void InverseTransformPoint(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (LinearTransformation, Vector3) input)
        {
            TransformationAssert.InverseTransformPoint(input);
        }

        #endregion


        #region Transformation of Directions

        [Test]
        public void TransformDirection(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (LinearTransformation, Vector3) input)
        {
            TransformationAssert.TransformDirection(input);
        }

        [Test]
        public void InverseTransformDirection(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (LinearTransformation, Vector3) input)
        {
            TransformationAssert.TransformDirection(input);
        }

        #endregion
    }
}