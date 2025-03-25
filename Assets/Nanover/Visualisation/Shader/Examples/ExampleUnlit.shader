/// Example unlit shader. This shader draws an opaque object with a solid color without instancing or shadow support

Shader "NanoverIMD/Example/Unlit"
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

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            struct fout
            {
                float4 color : SV_Target;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fout frag (v2f i) : SV_Target
            {
                fout o;
                o.color = _Color;
                return o;
            }
            
            ENDCG
        }
    }
}
