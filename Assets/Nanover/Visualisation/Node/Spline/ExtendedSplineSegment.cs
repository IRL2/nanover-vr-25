using System;
using UnityEngine;

namespace Nanover.Visualisation.Node.Spline
{
    /// <summary>
    /// This data structure is nearly identical to the base <c>SplineSegment</c> structure that it
    /// is based on. However, this introduces two new fields <c>endRadius</c> & <c>startRadius</c>
    /// which are used to control the size of each spline segment. Note that contrary to what their
    /// names might suggest the <c>startScale</c> and <c>endScale</c> fields actually effect the
    /// "width" of the segments and have no effect on loops. Thus, there is the need to introduce a
    /// new field which allows for true scaling of each spline segment. This is linked to the C#
    /// class of the same name. This is used by the <c>ExtendedTetrahedral</c> shader to render
    /// variable width residue spline segments.
    /// </summary>

    [Serializable]
    public struct ExtendedSplineSegment
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

        public float StartRadius;
        public float EndRadius;

        public Vector3 GetPoint(float t)
        {
            return (2 * t * t * t - 3 * t * t + 1) * StartPoint
                 + (t * t * t - 2 * t * t + t) * StartTangent
                 + (-2 * t * t * t + 3 * t * t) * EndPoint
                 + (t * t * t - t * t) * EndTangent;
        }
    }
}