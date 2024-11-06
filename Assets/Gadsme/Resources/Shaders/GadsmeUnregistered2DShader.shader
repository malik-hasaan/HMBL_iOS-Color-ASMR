Shader "Gadsme/Unregistered2DShader"
{
    Properties
    {
        _CornerRadius ("Corner Radius", Float) = 0.0
        _Resolution ("Resolution", Vector) = (300,250,0,0)
    }

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

            float _CornerRadius;
            float2 _Resolution;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _antialiasedCutoff (float distance) {
                float distanceChange = fwidth(distance) * 0.5;
                return smoothstep(distanceChange, -distanceChange, distance);
            }

            float _rectangle (float2 samplePosition, float2 halfSize) {
                float2 distanceToEdge = abs(samplePosition) - halfSize;
                float outsideDistance = length(max(distanceToEdge, 0));
                float insideDistance = min(max(distanceToEdge.x, distanceToEdge.y), 0);
                return outsideDistance + insideDistance;
            }

            float _roundedRectangle (float2 samplePosition, float absoluteRound, float2 halfSize) {
                return _rectangle(samplePosition, halfSize - absoluteRound) - absoluteRound;
            }

            float roundedRect (float2 samplePosition, float2 size, float radius) {
                float2 samplePositionTranslated = (samplePosition - .5) * size;
                float distToRect = _roundedRectangle(samplePositionTranslated, radius, size * .5);
                return _antialiasedCutoff(distToRect);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float pos = lerp(i.uv.x, i.uv.y * _Resolution.y / _Resolution.x, -6);
                fixed value = floor(frac(pos) + 0.5);

                float radius = (_CornerRadius / _Resolution.x);
                float2 rect = float2(1.0, _Resolution.y / _Resolution.x);
                float visible = roundedRect(i.uv, rect, radius);

                half4 pixel = lerp(float4(1, 1, 1, 0), float4(1, 1, 1, 0.3), value);
                pixel.a *= visible;

                return pixel;
            }
            ENDCG
        }
    }
}
