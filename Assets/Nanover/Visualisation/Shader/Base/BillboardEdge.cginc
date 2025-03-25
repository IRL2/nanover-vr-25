// Shader code to render quads that are billboarded, but aligned along a given axis

float _EdgeScale;
float4 _Color;

// Width of the gradient
float _GradientWidth;

#include "UnityCG.cginc"

#include "../Instancing.cginc"
#include "../Transformation.cginc"

void setup() {
    
}

struct appdata
{
    float4 vertex : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float4 color1 : TEXCOORD2;
    float4 color2 : TEXCOORD3;
    
    // Lerp value from 0 (start) to 1 (end)
    float lerp :TEXCOORD1;
};

v2f vert (appdata i)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(i);
    
    #if defined(PROCEDURAL_INSTANCING_ON)
        // Use instanced position and scale
        float3 startPosition = edge_position(0);
        float3 endPosition = edge_position(1);
        float scale = _EdgeScale;
        setup_billboard_edge_transformation_y(startPosition, endPosition, scale);
    #else
        // Use object position and scale
        float3 position = unity_ObjectToWorld._14_24_34;
        float3 a = 0.5 * unity_ObjectToWorld._12_22_32;
        
        float scale = length(unity_ObjectToWorld._11_21_31);
        
        setup_billboard_edge_transformation_y(position - a, position + a, scale);
         
    #endif
    
    o.vertex = UnityObjectToClipPos(i.vertex);

    o.color1 = _Color * edge_color(0);
    o.color2 = _Color * edge_color(1);
    
    o.lerp = 0.5 + 0.5 * i.vertex.y;
    
    return o;
}

struct fout {
    float4 color : SV_Target;
};

fout frag (v2f i)
{
    fout o;
    
    float lerpt = clamp((i.lerp - 0.5) / (_GradientWidth + 0.0001) + 0.5, 0, 1);

    o.color = lerp(i.color1, i.color2, lerpt);
    return o;
}