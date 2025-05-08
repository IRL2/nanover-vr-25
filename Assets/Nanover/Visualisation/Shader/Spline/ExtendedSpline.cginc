/// Contains methods for dealing with splines

#ifndef EXTENDED_SPLINE_CGINC_INCLUDED

    #define EXTENDED_SPLINE_CGINC_INCLUDED
    /*
     * This data structure is nearly identical to the base `SplineSegment` structure that it is
     * based on. However, this introduces two new fields `endRadius` and `startRadius` which are
     * used to control the size of each spline segment. Note that contrary to what their names might
     * suggest the `startScale` and `endScale` fields actually effect the "width" of the segments
     * and have no effect on loops. Thus, there is the need to introduce a new field which allows
     * for true scaling of each spline segment. This is linked to the C# class of the same name.
     * This is used by the `ExtendedTetrahedral` shader to render variable width residue spline
     * segments.
     */
    struct ExtendedSplineSegment {
        float3 startPoint;
        float3 endPoint;
        float3 startTangent;
        float3 endTangent;
        float3 startNormal;
        float3 endNormal;
        float4 startColor;
        float4 endColor;
        float3 startScale;
        float3 endScale;
        float startRadius;
        float endRadius;
    };
    
    StructuredBuffer<ExtendedSplineSegment> SplineArray;

    ExtendedSplineSegment instance_spline() {
        return SplineArray[instance_id];
    }
    
    float3 GetHermitePoint(float t, float3 startPoint, float3 startTangent, float3 endPoint, float3 endTangent)
    {
        return (2 * t * t * t - 3 * t * t + 1) * startPoint + (t * t * t - 2 * t * t + t) * startTangent +
               (-2 * t * t * t + 3 * t * t) * endPoint + (t * t * t - t * t) * endTangent;
    }

    float3 GetHermiteTangent(float t, float3 startPoint, float3 startTangent, float3 endPoint, float3 endTangent)
    {
        return (6 * t * t - 6 * t) * startPoint + (3 * t * t - 4 * t + 1) * startTangent +
               (-6 * t * t + 6 * t) * endPoint + (3 * t * t - 2 * t) * endTangent;
    }
    
    float3 GetHermiteSecondDerivative(float t, float3 startPoint, float3 startTangent, float3 endPoint, float3 endTangent)
    {
        return (12 * t - 6) * startPoint + (6 * t - 4) * startTangent +
               (-12 * t + 6) * endPoint + (6 * t - 2) * endTangent;
    }

#endif
