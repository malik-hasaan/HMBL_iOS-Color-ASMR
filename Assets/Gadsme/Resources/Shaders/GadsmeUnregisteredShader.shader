Shader "Gadsme/UnregisteredShader"
{
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

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
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = o.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float pos = lerp(i.uv.x, i.uv.y, -1);
                fixed value = floor(frac(pos) + 0.5);
                return lerp(float4(1, 1, 1, 0), float4(1, 1, 1, 0.3), value);
            }
            ENDCG
        }
    }
}
