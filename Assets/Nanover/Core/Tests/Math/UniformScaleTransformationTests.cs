using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Math;
using Nanover.Testing;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace Nanover.Core.Tests.Math
{
    public class UniformScaleTransformationTests
    {
        private static readonly int TestCount = 128;

        public static IEnumerable<UniformScaleTransformation> GetTransformations()
        {
            return SpatialTestData.GetRandomTransformationsUniformScale(TestCount, 87543);
        }

        public static IEnumerable<(UniformScaleTransformation, UniformScaleTransformation)>
            GetTransformationPairs()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomTransformationsUniformScale(TestCount, 325),
                SpatialTestData.GetRandomTransformationsUniformScale(TestCount, 754321),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(UniformScaleTransformation, Vector3)>
            GetTransformationAndVectors()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomTransformationsUniformScale(TestCount, 8653),
                SpatialTestData.GetRandomPositions(TestCount, 23606),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(UniformScaleTransformation, Quaternion)>
            GetTransformationAndRotations()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomTransformationsUniformScale(TestCount, 24794),
                SpatialTestData.GetRandomRotations(TestCount, 12075),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(UniformScaleTransformation, float)>
            GetTransformationAndScales()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomTransformationsUniformScale(TestCount, 236),
                SpatialTestData.GetRandomNonZeroScaleFactors(TestCount, 8562),
                (a, b) => (a, b)
            );
        }

        public static IEnumerable<(Vector3, Quaternion, float)> GetTranslationRotationAndScales()
        {
            return Enumerable.Zip(
                Enumerable.Zip(
                    SpatialTestData.GetRandomPositions(TestCount, 325),
                    SpatialTestData.GetRandomRotations(TestCount, 8568),
                    (a, b) => (a, b)
                ),
                SpatialTestData.GetRandomNonZeroScaleFactors(TestCount, 63464),
                (a, b) => (a.Item1, a.Item2, b)
            );
        }
        
        public static IEnumerable<(UniformScaleTransformation, Transformation)>
            GetTransformationAndTRSs()
        {
            return Enumerable.Zip(
                SpatialTestData.GetRandomTransformationsUniformScale(TestCount, 43734),
                SpatialTestData.GetRandomTransformations(TestCount, 32643),
                (a, b) => (a, b)
            );
        }

        #region Constructors

        [Test]
        public void Constructor(
            [ValueSource(nameof(GetTranslationRotationAndScales))]
            (Vector3, Quaternion, float) input)
        {
            var (position, rotation, scale) = input;
            var transformation = new UniformScaleTransformation(position, rotation, scale);
            MathAssert.AreEqual(position, transformation.position);
            MathAssert.AreEqual(rotation, transformation.rotation);
            MathAssert.AreEqual(scale, transformation.scale);
        }

        #endregion


        #region Constants

        [Test]
        public void Identity()
        {
            Assert.AreEqual(Matrix4x4.identity, UniformScaleTransformation.identity.matrix);
        }

        #endregion


        #region Inverse

        [Test]
        public void Inverse(
            [ValueSource(nameof(GetTransformations))] UniformScaleTransformation transformation)
        {
            TransformationAssert.IsInverseCorrect(transformation);
        }

        #endregion


        #region Matrices

        [Test]
        public void PositionFromMatrix(
            [ValueSource(nameof(GetTransformationAndVectors))] (UniformScaleTransformation, Vector3) input)
        {
            var (transformation, position) = input;
            transformation.position = position;
            MathAssert.AreEqual(position, transformation.matrix.GetTranslation());
        }

        [Test]
        public void RotationFromMatrix(
            [ValueSource(nameof(GetTransformationAndRotations))] (UniformScaleTransformation, Quaternion) input)
        {
            var (transformation, rotation) = input;
            if (transformation.scale < 0)
                return; // Cannot extract rotations from matrices involving negative scales.
            transformation.rotation = rotation;
            MathAssert.AreEqual(rotation, transformation.matrix.GetRotation());
        }

        [Test]
        public void ScaleFromMatrix(
            [ValueSource(nameof(GetTransformationAndScales))] (UniformScaleTransformation, float) input)
        {
            var (transformation, scale) = input;
            if (scale < 0)
                return; // Cannot extract scales from matrices involving negative scales.
            transformation.scale = scale;
            MathAssert.AreEqual(scale * Vector3.one, transformation.matrix.GetScale());
        }

        [Test]
        public void InverseMatrix(
            [ValueSource(nameof(GetTransformations))] UniformScaleTransformation transformation)
        {
            TransformationAssert.IsInverseMatrixCorrect(transformation);
        }

        #endregion


        #region Conversions

        [Test]
        public void AsMatrix(
            [ValueSource(nameof(GetTransformations))] UniformScaleTransformation transformation)
        {
            MathAssert.AreEqual(transformation.matrix, (Matrix4x4) transformation);
        }

        [Test]
        public void AsTransformation(
            [ValueSource(nameof(GetTransformations))] UniformScaleTransformation transformation)
        {
            MathAssert.AreEqual(transformation.matrix, ((Transformation) transformation).Matrix);
        }

        #endregion


        #region Multiplication

        [Test]
        public void Multiplication(
            [ValueSource(nameof(GetTransformationPairs))]
            (UniformScaleTransformation, UniformScaleTransformation) input)
        {
            TransformationAssert.Multiplication(input, input.Item1 * input.Item2);
        }

        #endregion


        #region Transformation of Points

        [Test]
        public void TransformPoint(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UniformScaleTransformation, Vector3) input)
        {
            TransformationAssert.TransformPoint(input);
        }

        [Test]
        public void InverseTransformPoint(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UniformScaleTransformation, Vector3) input)
        {
            TransformationAssert.InverseTransformPoint(input);
        }

        #endregion


        #region Transformation of Directions

        [Test]
        public void TransformDirection(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UniformScaleTransformation, Vector3) input)
        {
            TransformationAssert.TransformDirection(input);
        }

        [Test]
        public void InverseTransformDirection(
            [ValueSource(nameof(GetTransformationAndVectors))]
            (UniformScaleTransformation, Vector3) input)
        {
            TransformationAssert.InverseTransformDirection(input);
        }

        #endregion


        #region TransformPose

        [Test]
        public void TransformationTo(
            [ValueSource(nameof(GetTransformationAndTRSs))]
            (UniformScaleTransformation, Transformation) input)
        {
            var (transformation, trs) = input;
            var conversion = transformation.TransformationTo(trs);
            MathAssert.AreEqual(transformation.matrix.inverse * trs.Matrix, conversion.Matrix);
        }

        [Test]
        public void TransformBy(
            [ValueSource(nameof(GetTransformationAndTRSs))]
            (UniformScaleTransformation, Transformation) input)
        {
            var (transformation, trs) = input;
            var conversion = transformation.TransformationTo(trs);
            MathAssert.AreEqual(trs.Matrix, transformation.TransformBy(conversion).Matrix);
        }

        #endregion
    }
}