using Nanover.Testing;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools.Utils;

namespace Nanover.Core.Tests.Math
{
    public class MathAssert
    {
        /// <summary>
        /// Assert that two matrices are equal.
        /// </summary>
        public static void AreEqual(Matrix4x4 expected, Matrix4x4 actual, float error = 1e-3f)
        {
            var comparer = new Matrix4x4RowEqualityComparer(error);
            Assert.That(actual,
                        Is.EqualTo(expected)
                          .Using(comparer));
        }

        /// <summary>
        /// Assert that two quaternions are equal.
        /// </summary>
        public static void AreEqual(Quaternion expected, Quaternion actual, float error = 1e-3f)
        {
            var comparer = new QuaternionEqualityComparer(error);
            Assert.That(actual,
                        Is.EqualTo(expected)
                          .Using(comparer));
        }
        
        /// <summary>
        /// Assert that two vectors are equal.
        /// </summary>
        public static void AreEqual(Vector3 expected, Vector3 actual, float error = 1e-3f)
        {
            var comparer = new Vector3EqualityComparer(error);
            Assert.That(actual,
                        Is.EqualTo(expected)
                          .Using(comparer));
        }
        
        /// <summary>
        /// Assert that two floats are equal.
        /// </summary>
        public static void AreEqual(float expected, float actual, float error = 1e-3f)
        {
            var comparer = new FloatEqualityComparer(error);
            Assert.That(actual,
                        Is.EqualTo(expected)
                          .Using(comparer));
        }
    }
}