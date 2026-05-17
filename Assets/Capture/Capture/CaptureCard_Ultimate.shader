Shader "UI/CaptureCard_Ultimate"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _RangeMode ("Range Mode (0=None, 1=LimToFull, 2=FullToLim)", Float) = 0
        _Brightness ("Brightness", Range(-1, 1)) = 0.0
        _Contrast ("Contrast", Range(0, 2)) = 1.0
        _Saturation ("Saturation", Range(0, 2)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color    : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _RangeMode;
            float _Brightness;
            float _Contrast;
            float _Saturation;

            v2f vert(appdata_t v) {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target {
                fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // 1. Color Range Fix (Limited <-> Full)
                if (_RangeMode > 0.5 && _RangeMode < 1.5) {
                    col.rgb = (col.rgb - (16.0/255.0)) * (255.0/(235.0-16.0));
                }
                else if (_RangeMode > 1.5) {
                    col.rgb = col.rgb * (219.0/255.0) + (16.0/255.0);
                }

                // 2. Brightness (Additiv)
                col.rgb += _Brightness;

                // 3. Contrast (Skalierung um den Mittelpunkt 0.5)
                col.rgb = (col.rgb - 0.5f) * _Contrast + 0.5f;

                // 4. Saturation
                float luminance = dot(col.rgb, float3(0.2126, 0.7152, 0.0722));
                col.rgb = lerp(float3(luminance, luminance, luminance), col.rgb, _Saturation);

                // Sicherheitshalber Werte begrenzen (0 bis 1)
                col.rgb = saturate(col.rgb);
                return col;
            }
            ENDCG
        }
    }
}