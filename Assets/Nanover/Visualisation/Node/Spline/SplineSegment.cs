using System;
using UnityEngine;

namespace Nanover.Visualisation.Node.Spline
{
    [Serializable]
    public struct SplineSegment
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public Vector3 StartTangent;
        public Vector3 EndTangent;
        public Vector3 StartNormal;
        public Vector3 EndNormal;

        public UnityEngine.Color StartColor;
        public UnityEngine.Color EndColor;
        public Vector3 StartScale;
        public Vector3 EndScale;

        public Vector3 GetPoint(float t)
        {
            return (2 * t * t * t - 3 * t * t + 1) * StartPoint
                 + (t * t * t - 2 * t * t + t) * StartTangent
                 + (-2 * t * t * t + 3 * t * t) * EndPoint
                 + (t * t * t - t * t) * EndTangent;
        }
    }
}