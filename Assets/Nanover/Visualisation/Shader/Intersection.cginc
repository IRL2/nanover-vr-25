/// Contains methods for computing intersections

#ifndef INTERSECTION_CGINC_INCLUDED

    #define INTERSECTION_CGINC_INCLUDED
    
    // For a ray of the form r = q + d t, solve to find the first point where the ray has a length s from the origin by solving
    //
    //   r.r = s^2    =>    (q + d t).(q + d t) = s^2
    //
    // This returns the first intersection (using q + d t) and the corresponding t.
    // If this never occurs, this function discards the pixel.
    float4 solve_ray_intersection(float3 q, float3 d, float s) {
        float qq = dot(q, q) - s*s;
        float dq = dot(d, q);
        float dd = dot(d, d);
        
        float b2a = dq / dd;
        float ca = qq / dd;
        
        float det = b2a * b2a - ca;
        
        if(det < 0)
            discard;
            
        float t = -b2a - sqrt(det);
        
        float3 r = q + d * t;
        
        return float4(r, t);
    };
    
    // For a ray of the form r = q + d t, solve to find the first point where the ray has a length s from the origin by solving
    //
    //   r.r = s^2    =>    (q + d t).(q + d t) = s^2
    //
    // This returns the first intersection (using q + d t) and the corresponding t.
    // If this never occurs, this function discards the pixel.
    int is_ray_intersection(float3 q, float3 d, float s, out float t) {
        float qq = dot(q, q) - s*s;
        float dq = dot(d, q);
        float dd = dot(d, d);
        
        float b2a = dq / dd;
        float ca = qq / dd;
        
        float det = b2a * b2a - ca;
        
        if(det < 0) {
            t = 0;
            return 0;
        }
            
        t = -b2a - sqrt(det);
        
        return 1;
    };
    
    // Reject a given vector from an axis, giving the pependicular distance from the axis
    float3 reject(float3 v, float3 axis) {
        return v - dot(v, axis) / dot(axis, axis) * axis;
    };

#endif
