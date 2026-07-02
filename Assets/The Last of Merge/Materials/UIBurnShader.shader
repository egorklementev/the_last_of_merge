Shader "UI/BurningPaper"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Strength ("Burn Strength (0-1)", Range(0,1)) = 0
        _BurnSpeed ("Burn Speed", Range(0.1, 2)) = 1
        _EdgeGlow ("Edge Glow Intensity", Range(0, 2)) = 1
        _NoiseScale ("Noise Scale", Range(1, 10)) = 3
        [MaterialToggle] _UseNoise ("Use Noise Pattern", Float) = 1
        
        [StencilComp] _StencilComp ("Stencil Comparison", Float) = 8
        [Stencil] _Stencil ("Stencil ID", Float) = 0
        [StencilOp] _StencilOp ("Stencil Operation", Float) = 0
        [StencilWriteMask] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [StencilReadMask] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            float _Strength;
            float _BurnSpeed;
            float _EdgeGlow;
            float _NoiseScale;
            float _UseNoise;

            // Simple noise function
            float2 hash(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }

            float noise(float2 p)
            {
                const float K1 = 0.975805;
                const float K2 = 0.291286;
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float n = lerp(
                    lerp(dot(hash(i + float2(0.0, 0.0)), f - float2(0.0, 0.0)),
                         dot(hash(i + float2(1.0, 0.0)), f - float2(1.0, 0.0)), u.x),
                    lerp(dot(hash(i + float2(0.0, 1.0)), f - float2(0.0, 1.0)),
                         dot(hash(i + float2(1.0, 1.0)), f - float2(1.0, 1.0)), u.x), u.y);
                return 0.5 + 0.5 * n;
            }

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half2 uv = IN.texcoord;
                
                // Sample the original texture first
                fixed4 texColor = (tex2D(_MainTex, uv) + _TextureSampleAdd) * IN.color;
                
                // If strength is 0 or very close, return original color immediately
                if (_Strength < 0.001)
                {
                    // Apply UI clip rect
                    #ifdef UNITY_UI_CLIP_RECT
                    float alpha = texColor.a * UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                    #else
                    float alpha = texColor.a;
                    #endif
                    
                    #ifdef UNITY_UI_ALPHACLIP
                    clip(alpha - 0.001);
                    #endif
                    
                    return fixed4(texColor.rgb, alpha);
                }
                
                // Calculate distance from edges (0 at edges, 1 at center)
                float distFromEdge = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y)) * 2.0;
                
                // Add noise for organic burn pattern
                float noiseVal = _UseNoise > 0.5 ? noise(uv * _NoiseScale * 10.0) : 0.5;
                float burnThreshold = _Strength * _BurnSpeed;
                
                // Combine distance and noise for burn pattern
                float burnProgress = distFromEdge + noiseVal * 0.3 - burnThreshold;
                
                // Burn edge colors
                fixed3 burnColorDark = fixed3(0.1, 0.05, 0.0); // Dark brown/black
                fixed3 burnColorGlow = fixed3(1.0, 0.4, 0.1);   // Orange glow
                fixed3 burnColorEmber = fixed3(0.8, 0.2, 0.0);  // Red ember
                
                // Determine burn state
                float burnEdge = smoothstep(0.1, 0.3, burnProgress);
                float burnCore = smoothstep(-0.1, 0.1, burnProgress);
                float burnGlow = smoothstep(0.0, 0.2, burnProgress) * (1.0 - smoothstep(0.2, 0.4, burnProgress));
                
                // Mix colors based on burn state
                fixed3 finalColor = lerp(burnColorDark, texColor.rgb, burnEdge);
                finalColor = lerp(finalColor, burnColorEmber, (1.0 - burnCore) * 0.5);
                
                // Add glow at burn edge
                float glowIntensity = burnGlow * _EdgeGlow * (1.0 - _Strength);
                finalColor += burnColorGlow * glowIntensity;
                
                // Calculate alpha
                float alpha = texColor.a;
                alpha *= smoothstep(0.0, 0.3, burnProgress);
                alpha *= (1.0 - smoothstep(0.9, 1.0, _Strength));
                
                // Apply UI clip rect
                #ifdef UNITY_UI_CLIP_RECT
                alpha *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(alpha - 0.001);
                #endif

                return fixed4(finalColor, alpha);
            }
            ENDCG
        }
    }
}