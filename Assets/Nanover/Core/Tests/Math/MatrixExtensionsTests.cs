using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Math;
using Nanover.Testing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;
using MatrixPair = System.Tuple<UnityEngine.Matrix4x4, UnityEngine.Matrix4x4>;

namespace Nanover.Core.Tests.Math
{
    internal class MatrixExtensionsTests
    {
        private static MatrixPair GenerateRandomMatrixPair()
        {
            return new MatrixPair(SpatialTestData.GetRandomTransformation().Matrix,
                                  SpatialTestData.GetRandomTransformation().Matrix);
        }

        private static MatrixPair GenerateRandomMatrixPairUniformScale()
        {
            return new MatrixPair(SpatialTestData.GetRandomTransformationPositiveUniformScale().Matrix,
                                  SpatialTestData.GetRandomTransformationPositiveUniformScale().Matrix);
        }

        private static IEnumerable<Transformation> RandomTransformation =>
            SpatialTestData.RandomTransformation.Take(16);

        private static IEnumerable<MatrixPair> RandomMatrixPairs =>
            RandomTestData.SeededRandom(GenerateRandomMatrixPair).Take(16);

        private static IEnumerable<MatrixPair> RandomMatrixPairsUniformScale => RandomTestData
                                                                                .SeededRandom(
                                                                                    GenerateRandomMatrixPairUniformScale)
                                                                                .Take(16);

        [Test]
        public void GetTranslation_OfRandomTRS_ReturnsInputTranslation(
            [ValueSource(nameof(RandomTransformation))] Transformation transformation)
        {
            Assert.That(transformation.Position,
                        Is.EqualTo(transformation.Matrix.GetTranslation())
                          .Using(Vector3EqualityComparer.Instance));
        }

        [Test]
        public void GetRotation_OfRandomTRS_ReturnsInputRotation(
            [ValueSource(nameof(RandomTransformation))] Transformation transformation)
        {
            var realRotation = transformation.Rotation;
            var testRotation = transformation.Matrix.GetRotation();

            Assert.That(realRotation,
                        Is.EqualTo(testRotation)
                          .Using(QuaternionEqualityComparer.Instance));
        }

        [Test]
        public void GetScale_WithScaleTRS_ReturnsInputScale(
            [ValueSource(nameof(RandomTransformation))] Transformation transformation)
        {
            Assert.That(transformation.Scale,
                        Is.EqualTo(transformation.Matrix.GetScale())
                          .Using(Vector3EqualityComparer.Instance));
        }

        [Test]
        public void GetRelativeTransformationTo_WithRandomTRS_TransformsAToB(
            [ValueSource(nameof(RandomMatrixPairs))] MatrixPair pair)
        {
            var transformation = pair.Item1.GetTransformationTo(pair.Item2);
            var transformed = pair.Item1.TransformedBy(transformation);

            Assert.That(pair.Item1.TransformedBy(transformation),
                        Is.EqualTo(pair.Item2)
                          .Using(Matrix4x4TRSEqualityComparer.Instance));
        }

        [Test]
        public void TransformedBy_WithRandomMatrixInverse_IsIdentity(
            [ValueSource(nameof(RandomTransformation))] Transformation transformation)
        {
            var matrix = transformation.Matrix;
            var compare = matrix.TransformedBy(matrix.inverse)
                                .TransformedBy(matrix);

            Assert.That(matrix,
                        Is.EqualTo(compare)
                          .Using(Matrix4x4TRSEqualityComparer.Instance));
        }
    }
}