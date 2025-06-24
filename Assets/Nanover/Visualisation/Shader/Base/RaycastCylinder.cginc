// Shader for drawing a cylinder along the y axis using raycasting. It can use procedural instancing
// to draw multiple cylinders between different points

// The computation for a cylinder-ray intersection involves:
//
//   p: the center of the cylinder
//   a: the axis of the cylinder
//   c: the camera position
//   v: the vertex of the bounding shape that has been hit
//   s: the radius of the cylinder
//
// The ray is defined as
//
//   d = v - c: the direction of the ray
//   c: the origin of the ray (the camera)
//
// The ray is w = c + d t. 
//
// The intersection occurs when the perpendicular distance of the ray from the cylinder is equal to the radius s
// It is worth considering the ray distance relative to the cylinder center
//
//   q = c - p: the ray origin relative to the cylinder center
//   r = w - p = q + d t: the ray relative to the cylinder center
//
// The values p, q and s are computed in the vertex shader and passed to the fragment shader.
//
// Here, we must solve the following:
//
//   reject(r).reject(r) = s*s
//
// Where reject(r) is
//
//   reject(r) = r - r.a / a.a * a
//
// The det(q, d, s) solves the generic quadratic
//
//    (q + d t).(q + d t) = s^2
//
// It returns the first intersection point and the corresponding value t. It discards the pixel if no intersection occurs.


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
};

v2f vert (appdata i)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(i);
    float overall_scale = length(ObjectToWorld._11_21_31);
    
    #if !defined(PROCEDURAL_INSTANCING_ON)
        
        
        float3 p = unity_ObjectToWorld._14_24_34;
         o.a = float3(0.5, 0, 0);
         float scale = 1;
    #else
        
        float3 p1 = edge_position(0);
        float3 p2 = edge_position(1);
        
        float scale = _EdgeScale;
        float3 off = normalize(p2 - p1);
    
       float rad1 = edge_scale(0) * _ParticleScale;
       float rad2 = edge_scale(1) * _ParticleScale;
    
       scale = min(min(rad1, rad2), scale) - 0.0001;
    
       float o1 = sqrt(rad1*rad1-scale*scale)/2;
       float o2 = sqrt(rad2*rad2-scale*scale)/2;
    
       p1 += off * o1;
       p2 -= off * o2;
       
        setup_isotropic_edge_transformation(p1, p2, scale);
        
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
   
    // Solve quadratic to get intersection point.
    float4 rt = solve_ray_intersection(reject(q, a), reject(d, a), s);
    
    // The rejected ray position
    float3 r_rejected = rt.xyz;
    float t = rt.w;
    
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
    o.color = DIFFUSE(lerp(i.color1, i.color2, lerpt), n, l, _Diffuse);
    OUTPUT_FRAG_DEPTH(o, c + d * t);
    return o;
}