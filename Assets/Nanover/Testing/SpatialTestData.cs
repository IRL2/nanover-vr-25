using System.Collections.Generic;
using System.Linq;
using Nanover.Core.Math;
using UnityEngine;

namespace Nanover.Testing
{
    public static class SpatialTestData
    {
        /// <summary>
        /// Maximum displacement along any axis to use in positional test data. The
        /// larger the coordinate, the fewer decimal places of precision remain
        /// accurate.
        /// </summary>
        public const float MaximumCoordinate = 10000;

        /// <summary>
        /// Maximum shrinking/enlargement factor to use in scaling test data. The
        /// larger the factor, the fewer decimal places of precision remain accurate.
        /// </summary>
        public const float MaximumScaleFactor = 100;

        /// <summary>
        /// Get a random position within the supported range of coordinates.
        /// Coordinates outside this range are too imprecise to be worth supporting.
        /// </summary>
        private static Vector3 GetRandomPosition()
        {
            return new Vector3(Random.Range(-MaximumCoordinate, MaximumCoordinate),
                               Random.Range(-MaximumCoordinate, MaximumCoordinate),
                               Random.Range(-MaximumCoordinate, MaximumCoordinate));
        }

        /// <summary>
        /// Get a uniformly random rotation.
        /// </summary>
        private static Quaternion GetRandomRotation()
        {
            return Random.rotation;
        }

        /// <summary>
        /// Get a random positive scale within the supported range of scales. Scales
        /// outside this range are too imprecise to be worth supporting, and not
        /// practically useful anyway.
        /// </summary>
        private static Vector3 GetRandomPositiveScale()
        {
            return new Vector3(GetRandomPositiveScaleFactor(),
                               GetRandomPositiveScaleFactor(),
                               GetRandomPositiveScaleFactor());
        }

        private static Vector3 GetRandomPositiveUniformScale()
        {
            return Vector3.one * GetRandomPositiveScaleFactor();
        }

        /// <summary>
        /// Get a random scale factor, scaled evenly between above 1 and below 1
        /// </summary>
        private static float GetRandomPositiveScaleFactor()
        {
            var range = Mathf.Log(MaximumScaleFactor, 2);

            return Mathf.Pow(2, Random.Range(-range, range));
        }

        private static float GetRandomNonZeroScaleFactor()
        {
            return GetRandomPositiveScaleFactor() * (GetRandomBool() ? 1f : -1f);
        }

        private static bool GetRandomBool()
        {
            return Random.value > 0.5f;
        }

        public static Transformation GetRandomTransformation()
        {
            var components = new Transformation
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomPositiveScale(),
            };

            return components;
        }

        public static Transformation GetRandomTransformationPositiveUniformScale()
        {
            var transformation = new Transformation
            {
                Position = GetRandomPosition(),
                Rotation = GetRandomRotation(),
                Scale = GetRandomPositiveUniformScale(),
            };

            return transformation;
        }

        private static UnitScaleTransformation GetRandomTransformationUnitScale()
        {
            return new UnitScaleTransformation(GetRandomPosition(),
                                               GetRandomRotation());
        }

        private static AffineTransformation GetRandomAffineTransformation()
        {
            return new AffineTransformation(GetRandomPositiveScaleFactor() * Random.onUnitSphere,
                                            GetRandomPositiveScaleFactor() * Random.onUnitSphere,
                                            GetRandomPositiveScaleFactor() * Random.onUnitSphere,
                                            GetRandomPosition());
        }

        private static LinearTransformation GetRandomLinearTransformation()
        {
            return new LinearTransformation(GetRandomPositiveScaleFactor() * Random.onUnitSphere,
                                            GetRandomPositiveScaleFactor() * Random.onUnitSphere,
                                            GetRandomPositiveScaleFactor() * Random.onUnitSphere);
        }

        private static UniformScaleTransformation GetRandomTransformationUniformScale()
        {
            return new UniformScaleTransformation(GetRandomPosition(),
                                                  GetRandomRotation(),
                                                  GetRandomNonZeroScaleFactor());
        }

        public static IEnumerable<UnitScaleTransformation> GetRandomTransformationsUnitScale(
            int n,
            int? seed = null)
        {
            return RandomTestData.SeededRandom(GetRandomTransformationUnitScale, seed).Take(n);
        }

        public static IEnumerable<AffineTransformation> GetRandomAffineTransformations(
            int n,
            int? seed = null)
        {
            return RandomTestData.SeededRandom(GetRandomAffineTransformation, seed).Take(n);
        }

        public static IEnumerable<LinearTransformation> GetRandomLinearTransformations(
            int n,
            int? seed = null)
        {
            return RandomTestData.SeededRandom(GetRandomLinearTransformation, seed).Take(n);
        }

        public static IEnumerable<UniformScaleTransformation> GetRandomTransformationsUniformScale(
            int n,
            int? seed = null)
        {
            return RandomTestData.SeededRandom(GetRandomTransformationUniformScale, seed).Take(n);
        }

        public static IEnumerable<Transformation> GetRandomTransformations(
            int n,
            int? seed = null)
        {
            return RandomTestData.SeededRandom(GetRandomTransformation, seed).Take(n);
        }

        public static IEnumerable<Vector3> GetRandomPositions(int n, int? seed = null)
        {
            return RandomTestData.SeededRandom(GetRandomPosition, seed).Take(n);
        }

        public static IEnumerable<Quaternion> GetRandomRotations(int n, int? seed = null)
        {
            return RandomTestData.SeededRandom(GetRandomRotation, seed).Take(n);
        }

        public static IEnumerable<float> GetRandomNonZeroScaleFactors(int n, int? seed = null)
        {
            return RandomTestData.SeededRandom(GetRandomNonZeroScaleFactor, seed).Take(n);
        }

        public static IEnumerable<Transformation> RandomTransformation =>
            RandomTestData.SeededRandom(GetRandomTransformation);
    }
}