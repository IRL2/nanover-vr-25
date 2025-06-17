// Shader for drawing multiple cylinders along the y axis using raycasting. It can use procedural instancing
// to draw multiple cylinders between different points

// Width of the gradient
float _GradientWidth;

// The scales to apply to the particles
float _ParticleScale;

// The scales to apply to the edges
float _EdgeScale;

// Color multiplier
float4 _Color;

// Diffuse factor (0 for flat, 1 for full diffuse)
float _Diffuse;

float _Spacing;

float _Shrink;

#include "UnityCG.cginc"
#include "../Instancing.cginc"
#include "../Transformation.cginc"
#include "../Intersection.cginc"
#include "../Depth.cginc"
#include "../Lighting.cginc"

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
    float4 q : TEXCOORD0;
    float4 d : TEXCOORD1;
    float4 color1 : TEXCOORD2;
    float4 color2 : TEXCOORD3;
    float3 a : TEXCOORD4;
    float4 b : TEXCOORD5;
};

v2f vert (appdata i)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(i);
    
    int N = max(1, instance_edge_count());
    
    float overall_scale = length(ObjectToWorld._11_21_31);
    
    float spacing = overall_scale * _Spacing * pow(_Shrink, N-1);
    
    #if !defined(PROCEDURAL_INSTANCING_ON)
        float3 p = unity_ObjectToWorld._14_24_34;
         o.a = float3(0.5, 0, 0);
         float scale = 1;
    #else
        float3 p1 = edge_position(0);
        float3 p2 = edge_position(1);
        
        float scale = _EdgeScale * pow(_Shrink, N-1);
        float3 off = normalize(p2 - p1);
        
        setup_billboard_edge_transformation_z(p1, p2, float2(scale+spacing/overall_scale*(N-1), scale));
        
        float3 p = 0.5 * (p1 + p2);
        
        o.a = 0.5 * (p2 - p1);
        
        p = mul(ObjectToWorld, float4(p, 1)).xyz;
        
        o.a = mul(ObjectToWorld, float4(o.a, 0)).xyz;
    #endif
    
    
    o.vertex = UnityObjectToClipPos(i.vertex);
    
    float3 v = mul(unity_ObjectToWorld, i.vertex);
    float3 c = _WorldSpaceCameraPos.xyz;
    
    float s = scale * 0.5 * overall_scale;
    
    o.q = float4(c - p, s);
    o.d = float4(v - c, 0);
    
    
    float3 xaxis = normalize(cross(o.a, c - p));
    o.b.xyz = xaxis * spacing;
    o.b.w = float(N)+0.001;
        
   
    o.color1 = _Color * edge_color(0);
    o.color2 = _Color * edge_color(1);
    return o;
}


struct fout {
    float4 color : SV_Target;
    float depth : SV_Depth;
};

fout frag (v2f i)
{
    fout o;
    float3 q = i.q.xyz;
    float3 d = i.d.xyz;
    float s = i.q.w;
    float3 a = i.a.xyz;
    float3 b = i.b.xyz;
   
    float t = 9999999;
    float t_new = 0;
    float3 r_rejected = 0;
    
    int N = floor(i.b.w);
    
    for(int j = 0; j < N; j++) {
        float3 q0 = reject(q + b * (j-float(N-1)/2.0), a);
        float3 d0 = reject(d, a);
        // Solve quadratic to get intersection point.
        int intersect = is_ray_intersection(q0, d0, s, t_new);
        if(intersect > 0 && t_new < t) {
            t = t_new;
            r_rejected = q0 + t * d0;
        }
    }
    
    if(t > 99999)
        discard;
    
    // Project the world space position q + d t onto the cylinder axis
    float proj = dot(q + d * t, a) / dot(a, a);
    
    // Remove any points which don't lie within the containing box
    if(proj * proj > 1.0)
        discard;
    
    float3 n = normalize(r_rejected);
    float3 l = normalize(_WorldSpaceLightPos0.xyz);
    float3 c = _WorldSpaceCameraPos.xyz;
    
    float lerpt = 0.5 + 0.5 * proj;
    
    lerpt = clamp((lerpt - 0.5) / (_GradientWidth + 0.0001) + 0.5, 0, 1);
    float4 color = lerp(i.color1, i.color2, lerpt);
    
    o.color = DIFFUSE(color, n, l, _Diffuse);
    o.color.a = 1.0;
    
    OUTPUT_FRAG_DEPTH(o, c + d * t);
    return o;
}