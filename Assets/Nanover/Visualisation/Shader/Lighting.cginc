/// Contains methods for lighting

#ifndef LIGHTING_CGINC_INCLUDED

    #define LIGHTING_CGINC_INCLUDED
    
    #define DIFFUSE(c, n, l, d) float4((c * saturate(lerp(1, dot(n, l), d))).rgb, 1.0);
    
#endif