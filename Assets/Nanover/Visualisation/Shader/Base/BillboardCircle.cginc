// Shader code to render circles billboarded towards the camera

// Scale multiplier for the circles
float _Scale;

// Color multiplier for the circles
float4 _Color;

#include "UnityCG.cginc"

#include "../Instancing.cginc"
#include "../Transformation.cginc"

void setup() {

}

struct appdata
{
    float4 vertex : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float4 color : TEXCOORD1;
    float2 uv : TEXCOORD0;
};

struct fout {
    float4 color : SV_Target;
};

v2f vert (appdata i)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(i);
    
    #if defined(PROCEDURAL_INSTANCING_ON)
        // Use instanced position and scale
        float3 position = instance_position();
        float3 scale = float3(1,1,1) * _Scale * instance_scale();
        setup_billboard_transformation(position, scale);
    #else
        // Use object position and scale
        float3 position = unity_ObjectToWorld._14_24_34;
        float3 scale = length(unity_ObjectToWorld._11_21_31);
        setup_billboard_transformation(position, scale);
    #endif
    
    o.vertex = UnityObjectToClipPos(i.vertex);
    
    // Get UV coordinates with center of quad being (0,0)
    o.uv = i.uv - 0.5;
  
    o.color = _Color * instance_color();
    
    return o;
}

fout frag (v2f i)
{
    fout o;
    
    // Discard pixel if outside of circle
    if(dot(i.uv, i.uv) > 0.25)
        discard;
    
    o.color = i.color;
    return o;
}
            