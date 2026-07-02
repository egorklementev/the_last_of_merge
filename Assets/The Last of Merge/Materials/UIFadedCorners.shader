Shader "UI/MultiplyTextures"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _MultiplyTex ("Multiply Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Lighting Off ZWrite Off ZTest Always Cull Off Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            sampler2D _MultiplyTex;
            fixed4 _Color;
            
            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };
            
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 mainTex = tex2D(_MainTex, i.uv) * i.color * _Color;
                fixed4 multiplyTex = tex2D(_MultiplyTex, i.uv);
                return mainTex * multiplyTex;
            }
            ENDCG
        }
    }
}