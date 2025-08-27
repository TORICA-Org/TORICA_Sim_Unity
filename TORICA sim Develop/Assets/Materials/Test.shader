Shader "Custom/FrostedGlass_Stable"
{
    Properties
    {
        [Header(Base Settings)]
        _Color ("Color Tint", Color) = (1,1,1,0.75)
        _MainTex ("Texture (RGBA)", 2D) = "white" {}
        _TextureAlpha ("Texture Opacity", Range(0, 1)) = 0.2

        [Header(Blur Settings)]
        _BlurSize ("Blur Amount", Range(0, 0.02)) = 0.005
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent+2" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // ▼▼▼ 修正点1: VR用のインスタンシングを有効にする宣言を追加 ▼▼▼
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl" // 念のため追加
            
            TEXTURE2D_X_FLOAT(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float _BlurSize;
                half _TextureAlpha;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert (Attributes v)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float2 screenUV = GetNormalizedScreenSpaceUV(i.positionHCS);

                // ▼▼▼ 修正点2: ぼかしの強さの計算を元に戻す ▼▼▼
                const int sampleCount = 9;
                const float2 offsets[sampleCount] = {
                    float2(-_BlurSize,  _BlurSize), float2(0,  _BlurSize), float2(_BlurSize,  _BlurSize),
                    float2(-_BlurSize,          0), float2(0,          0), float2(_BlurSize,          0),
                    float2(-_BlurSize, -_BlurSize), float2(0, -_BlurSize), float2(_BlurSize, -_BlurSize)
                };

                half4 blurColor = 0;
                for(int j = 0; j < sampleCount; j++)
                {
                    blurColor += SAMPLE_TEXTURE2D_X_LOD(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offsets[j], 0);
                }
                blurColor /= sampleCount;
                
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                texColor.a *= _TextureAlpha;

                half4 finalColor = lerp(blurColor, texColor, texColor.a);
                
                finalColor.rgb *= _Color.rgb;
                finalColor.a = _Color.a;

                return finalColor;
            }
            ENDHLSL
        }
    }
}