Shader "Gadsme/PlacementRoundedProgress"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _CornerRadius ("Corner Radius", Float) = 32.0
        _BorderThickness ("Border Thickness", Float) = 6.0
        _Resolution ("Resolution", Vector) = (300,250,0,0)
        _Progress ("Progress", Range(0, 1)) = 0.0
        _GaugeColor ("Gauge Color", Color) = (1,1,1,1)
        _GaugeBackgroundColor ("Gauge Background Color", Color) = (0,0,0,0.25)
    }

    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest [unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float _CornerRadius;
            float _BorderThickness;
            float2 _Resolution;
            float _Progress;
            half4 _GaugeColor;
            half4 _GaugeBackgroundColor;

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

            float cornerLength (float radius) {
                return 1.57079632679 * radius;
            }

            half4 frag (v2f i) : SV_Target
            {
                float quarter = 1.57079632679;
                float cornerLen = cornerLength(_CornerRadius);
                float totalPerimeter = _Resolution.x * 2 + _Resolution.y * 2 - _CornerRadius * 8 + cornerLen * 4;
                float consumedPerimeter = totalPerimeter * _Progress;

                float2 pos = float2(
                    i.uv.x * _Resolution.x,
                    (1.0 - i.uv.y) * _Resolution.y
                );

                fixed4 pixel = tex2D(_MainTex, i.uv);

                float radius = (_CornerRadius / _Resolution.x);

                float2 rect1 = float2(1.0, _Resolution.y / _Resolution.x);
                float inGauge1 = roundedRect(i.uv, rect1, radius);

                float2 thickness = float2(
                    _BorderThickness / _Resolution.x,
                    _BorderThickness / _Resolution.y
                );
                float2 rect2 = rect1;
                float2 uv2 = float2(
                    i.uv.x * (1.0 + thickness.x * 2) - thickness.x,
                    i.uv.y * (1.0 + thickness.y * 2) - thickness.y
                );
                float inGauge2 = roundedRect(uv2, rect2, radius - thickness.x * 0.5);

                pixel.a = inGauge1;

                float inProgress = 1.0;

                if (pos.x < _CornerRadius) {
                    if (pos.y < _CornerRadius) {
                        // Top left
                        float progress = (consumedPerimeter - _Resolution.x * 2 - _Resolution.y * 2 - cornerLen * 3 + _CornerRadius * 8) / cornerLen;
                        float angle = atan2(
                            (_CornerRadius - pos.y) / _CornerRadius,
                            (_CornerRadius - pos.x) / _CornerRadius
                        );
                        float pixelProgress = 1.0 - (angle / quarter);
                        inProgress = smoothstep((1.0 - progress) - 0.0001, (1.0 - progress) + 0.0001, pixelProgress);
                    }
                    else if (pos.y > _Resolution.y - _CornerRadius) {
                        // Bottom left
                        float progress = (consumedPerimeter - _Resolution.x * 2 - _Resolution.y - cornerLen * 2 + _CornerRadius * 6) / cornerLen;
                        float angle = atan2(
                            (pos.y - _Resolution.y + _CornerRadius) / _CornerRadius,
                            (_CornerRadius - pos.x) / _CornerRadius
                        );
                        float pixelProgress = angle / quarter;
                        inProgress = smoothstep((1.0 - progress) - 0.0001, (1.0 - progress) + 0.0001, pixelProgress);
                    }
                    else {
                        // Left
                        float progress = (consumedPerimeter - _Resolution.x * 2 - _Resolution.y - cornerLen * 3 + _CornerRadius * 6) / (_Resolution.y - _CornerRadius * 2);
                        float pixelProgress = (pos.y - _CornerRadius) / (_Resolution.y - _CornerRadius * 2);
                        inProgress = smoothstep((1.0 - progress) - 0.0001, (1.0 - progress) + 0.0001, pixelProgress);
                    }
                }
                else if (pos.x >= _Resolution.x - _CornerRadius) {
                    if (pos.y < _CornerRadius) {
                        // Top right
                        float progress = (consumedPerimeter - _Resolution.x + _CornerRadius * 2) / cornerLen;
                        float angle = atan2(
                            (_CornerRadius - pos.y) / _CornerRadius,
                            (pos.x - _Resolution.x + _CornerRadius) / _CornerRadius
                        );
                        float pixelProgress = angle / quarter;
                        inProgress = smoothstep((1.0 - progress) - 0.0001, (1.0 - progress) + 0.0001, pixelProgress);
                    }
                    else if (pos.y >= _Resolution.y - _CornerRadius) {
                        // Bottom right
                        float progress = (consumedPerimeter - _Resolution.x - _Resolution.y - cornerLen + _CornerRadius * 4) / cornerLen;
                        float angle = atan2(
                            (pos.y - _Resolution.y + _CornerRadius) / _CornerRadius,
                            (pos.x - _Resolution.x + _CornerRadius) / _CornerRadius
                        );
                        float pixelProgress = 1.0 - (angle / quarter);
                        inProgress = smoothstep((1.0 - progress) - 0.0001, (1.0 - progress) + 0.0001, pixelProgress);
                    }
                    else {
                        // Right
                        float progress = (consumedPerimeter - _Resolution.x + _CornerRadius * 2 - cornerLen) / (_Resolution.y - _CornerRadius * 2);
                        float pixelProgress = 1.0 - ((pos.y - _CornerRadius) / (_Resolution.y - _CornerRadius * 2));
                        inProgress = smoothstep((1.0 - progress) - 0.0001, (1.0 - progress) + 0.0001, pixelProgress);
                    }
                }
                else {
                    if (pos.y < _CornerRadius) {
                        // Top
                        float progress = consumedPerimeter / (_Resolution.x - _CornerRadius * 2);
                        float pixelProgress = 1.0 - ((pos.x - _CornerRadius) / (_Resolution.x - _CornerRadius * 2));
                        inProgress = smoothstep((1.0 - progress) - 0.0001, (1.0 - progress) + 0.0001, pixelProgress);
                    }
                    else if (pos.y >= _Resolution.y - _CornerRadius) {
                        // Bottom
                        float progress = (consumedPerimeter - _Resolution.x - _Resolution.y - cornerLen * 2 + _CornerRadius * 4) / (_Resolution.x - _CornerRadius * 2);
                        float pixelProgress = (pos.x - _CornerRadius) / (_Resolution.x - _CornerRadius * 2);
                        inProgress = smoothstep((1.0 - progress) - 0.0001, (1.0 - progress) + 0.0001, pixelProgress);
                    }
                    else {
                        // Center
                    }
                }

                half3 gaugeEmptyColor = lerp(pixel.rgb, _GaugeBackgroundColor.rgb, _GaugeBackgroundColor.a);
                half3 gaugeStateColor = lerp(gaugeEmptyColor, _GaugeColor.rgb, inProgress);
                pixel.rgb = lerp(gaugeStateColor, pixel.rgb, inGauge2);

                return pixel;
            }
            ENDCG
        }
    }
}
