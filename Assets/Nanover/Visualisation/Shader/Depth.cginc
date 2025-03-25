/// Contains methods for computing depth

#ifndef DEPTH_CGINC_INCLUDED

    #define DEPTH_CGINC_INCLUDED
    
   
    // Compute the depth of a fragment from world position
    // TODO: Make this work on the Oculus Quest correctly
    float calculateFragmentDepth(float3 worldPos){
        float4 depthVec = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0));
        return depthVec.z/depthVec.w;
    }
    
    #define OUTPUT_FRAG_DEPTH(output, pos) output.depth = calculateFragmentDepth(pos);

#endif
