Shader "UI/CaptureCard_ColorRange"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // 0 = None, 1 = Limited To Full, 2 = Full To Limited
        _RangeMode ("Range Mode", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            fixed4 _Color;
            sampler2D _MainTex;
            float _RangeMode;

            v2f vert(appdata_t v) {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target {
                fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // Limited (16-235) to Full (0-255)
                if (_RangeMode > 0.5 && _RangeMode < 1.5) {
                    col.rgb = (col.rgb - (16.0/255.0)) * (255.0/(235.0-16.0));
                }
                // Full (0-255) to Limited (16-235)
                else if (_RangeMode > 1.5) {
                    col.rgb = col.rgb * (219.0/255.0) + (16.0/255.0);
                }

                col.rgb = saturate(col.rgb);
                return col;
            }
            ENDCG
        }
    }
}