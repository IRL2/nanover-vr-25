using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Math;
using Nanover.Frontend.Manipulation;
using Nanover.Testing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;
using TransformationPair = System.Tuple<Nanover.Core.Math.Transformation, Nanover.Core.Math.Transformation>;

namespace Nanover.Frontend.Tests
{
    internal class TransformGesturesTests
    {
        public class ObjectTransformTestSequence
        {
            public Transformation InitialObjectTransformation;
            public List<TransformationPair> Sequence;
        }

        internal static TransformationPair GenerateRandomMatrixPair()
        {
            var transformation1 = SpatialTestData.GetRandomTransformation();
            transformation1.Scale = Vector3.one;
            var transformation2 = SpatialTestData.GetRandomTransformation();
            transformation2.Scale = Vector3.one;

            return new TransformationPair(transformation1, transformation2);
        }

        internal static ObjectTransformTestSequence GenerateObjectTransformTestSequence()
        {
            return new ObjectTransformTestSequence
            {
                InitialObjectTransformation = SpatialTestData.GetRandomTransformationPositiveUniformScale(),
                Sequence = Enumerable.Range(0, 8)
                                     .Select(_ => GenerateRandomMatrixPair())
                                     .ToList(),
            };
        }

        internal static IEnumerable<ObjectTransformTestSequence>
            RandomObjectTransformTestSequence => RandomTestData
                                                 .SeededRandom(GenerateObjectTransformTestSequence)
                                                 .Take(16);

        [Test]
        public void OnePointGesture_WithRandomInput_PreservesScale(
            [ValueSource(nameof(RandomObjectTransformTestSequence))]
            ObjectTransformTestSequence sequence)
        {
            var gesture = new OnePointTranslateRotateGesture();

            var referenceScale = sequence.InitialObjectTransformation.Scale;

            gesture.BeginGesture(sequence.InitialObjectTransformation,
                                 sequence.Sequence.First().Item1.AsUnitTransformWithoutScale());

            foreach (var pair in sequence.Sequence.Skip(1))
            {
                var transformation = gesture.UpdateControlPoint(pair.Item1.AsUnitTransformWithoutScale());

                Assert.That(transformation.Scale,
                            Is.EqualTo(referenceScale)
                              .Using(Vector3EqualityComparer.Instance));
            }
        }

        [Test]
        public void TwoPointGesture_WithRandomInput_ScalesOnRelativeSeparation(
            [ValueSource(nameof(RandomObjectTransformTestSequence))]
            ObjectTransformTestSequence sequence)
        {
            var gesture = new TwoPointTranslateRotateScaleGesture();

            var initialControlPoint1 = sequence.Sequence.First().Item1;
            var initialControlPoint2 = sequence.Sequence.First().Item2;
            var initialSeparation = Vector3.Distance(initialControlPoint1.Position,
                                                     initialControlPoint2.Position);
            var initialObjectTransformation = sequence.InitialObjectTransformation;
            var referenceScale = initialObjectTransformation.Scale.x;

            gesture.BeginGesture(initialObjectTransformation,
                                 initialControlPoint1.AsUnitTransformWithoutScale(),
                                 initialControlPoint2.AsUnitTransformWithoutScale());

            foreach (var pair in sequence.Sequence.Skip(1))
            {
                var updatedControlPoint1 = pair.Item1;
                var updatedControlPoint2 = pair.Item2;
                var updatedSeparation = Vector3.Distance(updatedControlPoint1.Position,
                                                         updatedControlPoint2.Position);
                var expectedScale = referenceScale * (updatedSeparation / initialSeparation);

                var transformation = gesture.UpdateControlPoints(updatedControlPoint1.AsUnitTransformWithoutScale(),
                                                                 updatedControlPoint2.AsUnitTransformWithoutScale());

                Assert.That(transformation.Scale,
                            Is.EqualTo(Vector3.one * expectedScale)
                              .Using(Vector3EqualityComparer.Instance));
            }
        }

        [Test]
        public void TwoPointGesture_WithRandomInput_PreservesPointsLocalPosition(
            [ValueSource(nameof(RandomObjectTransformTestSequence))]
            ObjectTransformTestSequence sequence)
        {
            var gesture = new TwoPointTranslateRotateScaleGesture();

            var initialControlPoint1 = sequence.Sequence.First().Item1;
            var initialControlPoint2 = sequence.Sequence.First().Item2;
            var initialObjectTransformation = sequence.InitialObjectTransformation;

            var worldToObject = initialObjectTransformation.Matrix.inverse;
            var objectControlPoint1 = worldToObject.MultiplyPoint3x4(initialControlPoint1.Position);
            var objectControlPoint2 = worldToObject.MultiplyPoint3x4(initialControlPoint2.Position);

            gesture.BeginGesture(initialObjectTransformation,
                                 initialControlPoint1.AsUnitTransformWithoutScale(),
                                 initialControlPoint2.AsUnitTransformWithoutScale());

            foreach (var pair in sequence.Sequence.Skip(1))
            {
                var updatedControlPoint1 = pair.Item1;
                var updatedControlPoint2 = pair.Item2;

                var updatedObjectTransformation =
                    gesture.UpdateControlPoints(updatedControlPoint1.AsUnitTransformWithoutScale(), updatedControlPoint2.AsUnitTransformWithoutScale());
                var updatedWorldToObject = updatedObjectTransformation.Matrix.inverse;

                var updatedObjectControlPoint1 =
                    updatedWorldToObject.MultiplyPoint3x4(updatedControlPoint1.Position);
                var updatedObjectControlPoint2 =
                    updatedWorldToObject.MultiplyPoint3x4(updatedControlPoint2.Position);

                Assert.That(objectControlPoint1,
                            Is.EqualTo(updatedObjectControlPoint1)
                              .Using(Vector3EqualityComparer.Instance));

                Assert.That(objectControlPoint2,
                            Is.EqualTo(updatedObjectControlPoint2)
                              .Using(Vector3EqualityComparer.Instance));
            }
        }
    }
}