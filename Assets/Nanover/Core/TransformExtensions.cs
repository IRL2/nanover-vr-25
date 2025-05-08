using UnityEngine;

namespace Nanover.Core
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Set the transform to be the identity in local space.
        /// </summary>
        public static void SetToLocalIdentity(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }
    }
}