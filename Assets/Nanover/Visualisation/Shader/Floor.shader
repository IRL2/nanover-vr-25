// Shader for drawing a grid on the floor

Shader "Unlit/Floor"
{
    Properties
    {
        _Frequency ("Frequency", Float) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _Falloff("Falloff", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 world : TEXCOORD1;
            };

            float _Frequency;
            float _Falloff;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.world = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = _Color;
                
                float2 cameraXZ = _WorldSpaceCameraPos.xz;
                float2 worldXZ = i.world.xz;
                
                float d = length(worldXZ - cameraXZ);
                                
                float ex = exp(-(d*d) / _Falloff);
                                
                float cs = max(cos(i.uv.x * _Frequency), cos(i.uv.y * _Frequency));
                
                col.a *= 1-step(cs, 0.9999-ex*0.012);
                
                col.a *= ex;
                
                return col;
            }
            ENDCG
        }
    }
}
