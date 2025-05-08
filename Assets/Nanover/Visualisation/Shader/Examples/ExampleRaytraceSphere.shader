/// Example of a simple raytraced sphere. This shader draws a sphere in object space with a solid color without instancing, 
/// shadow support or depth

Shader "NanoverIMD/Example/Raytrace Sphere"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            float4 _Color;
            
            struct Ray {
                float4 origin;
                float4 direction;
            };
            
            struct Sphere {
                float4 centre;
                float radius;
            };
            
            /// Calculate the intersection of a ray and a sphere, returning the number of intersections
            int get_sphere_intersections(Sphere sphere, Ray ray, out float2 intersection_offsets) {
            
                float4 q = ray.origin - sphere.centre;
                
                float qq = dot(q, q);
                float dq = dot(ray.direction, q);
                
                float determinant = dq * dq - qq + sphere.radius * sphere.radius;
                
                if(determinant < 0)
                    return 0;
                    
                intersection_offsets.x = -dq - sqrt(determinant);
                intersection_offsets.y = -dq + sqrt(determinant);
                
                return 2;
            };

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 clippos : SV_POSITION;
                float4 vertex : TEXCOORD0;
                float4 viewdir : TEXCOORD1;
            };
            
            struct fout
            {
                float4 color : SV_Target;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.clippos = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                o.viewdir = v.vertex - mul(unity_WorldToObject, float4(_WorldSpaceCameraPos.xyz, 1));
                return o;
            }

            fout frag (v2f i) : SV_Target
            {
                fout o;
                
                Ray ray;
                ray.origin = i.vertex;
                ray.direction = normalize(i.viewdir);
                
                ray.origin.w = 1;
                ray.direction.w = 0;
                
                Sphere sphere;
                sphere.centre = float4(0,0,0,1);
                sphere.radius = 0.5;
                
                float2 intersection;
                if(!get_sphere_intersections(sphere, ray, intersection))
                    discard;
                
                o.color = _Color;
                return o;
            }
            
            ENDCG
        }
    }
}
