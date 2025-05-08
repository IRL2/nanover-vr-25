// Simple shader for a transparent ringed sphere, used for tool cursors.

Shader "Nanover iMD/Gizmo"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 normal : TEXCOORD0;
                float3 view : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, float4(v.normal.xyz, 0));
                o.view = _WorldSpaceCameraPos - mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1)).xyz;
                return o;
            }
            
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color;
                float d = dot(normalize(i.view.xyz), normalize(i.normal.xyz));
                d = 1 - d;
                col.a *=  d * d * d * d;
                return col;
            }
            ENDCG
        }
    }
}
