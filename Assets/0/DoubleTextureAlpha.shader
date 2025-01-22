Shader "Custom/DoubleTextureAlpha"
{
    Properties
    {
        _MainTex ("Base Color (RGB)", 2D) = "white" {}
        _AlphaTex ("Alpha Mask (R)", 2D) = "white" {}
        _ColorTint ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        // 设置为透明队列以便进行混合
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100
        
        // 开启Alpha混合 (SRC_ALPHA, 1-SRC_ALPHA)
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            Cull Off

            // 透明混合模式，前面：srcAlpha，背面：1-srcAlpha
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4    _MainTex_ST;   // 让 Unity 能够对该贴图进行 Tiling/Offset
            sampler2D _AlphaTex;
            float4    _AlphaTex_ST;
            
            float4    _ColorTint;    // 用于整体调节颜色，或只用白色(1,1,1,1)

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // TRANSFORM_TEX宏会对uv应用材质里设置的 Tiling / Offset
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样颜色贴图并乘以 Tint
                fixed4 col = tex2D(_MainTex, i.uv) * _ColorTint;

                // 你也可以改成 alphaSample.a 或其他通道
                fixed4 alphaSample = tex2D(_AlphaTex, i.uv);
                col.a = alphaSample.a;
                
                return col;
            }
            ENDCG
        }
    }
}
