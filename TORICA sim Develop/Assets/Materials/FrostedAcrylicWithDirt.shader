Shader "Custom/FrostedGlassURP"
{
    Properties
    {
        [Header(Base Settings)]
        _Color ("Color Tint", Color) = (1,1,1,0.5)
        _MainTex ("Texture (RGBA)", 2D) = "white" {}
        _TextureAlpha ("Texture Opacity", Range(0, 1)) = 0.2

        [Header(Blur Settings)]
        _BlurSize ("Blur Amount", Range(0, 0.01)) = 0.005
        _BlurSamples ("Blur Quality", Range(1, 8)) = 4
    }
    SubShader
    {
        // レンダリングパイプラインとキューを設定
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        Pass
        {
            // 半透明の描画設定
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // URPのコアライブラリと背景テクスチャ宣言をインクルード
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // URP 12以降で _CameraOpaqueTexture を使用するための宣言
            TEXTURE2D_X_FLOAT(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            // 頂点シェーダーの入力構造体
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            // フラグメントシェーダーへの受け渡し用構造体
            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 screenPos    : TEXCOORD1; // 背景テクスチャサンプリング用
            };

            // マテリアルのプロパティ
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half4 _Color;
                float _BlurSize;
                half _TextureAlpha;
                int _BlurSamples;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // 頂点シェーダー
            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // 背景テクスチャをサンプリングするためのスクリーン座標を計算
                o.screenPos = ComputeScreenPos(o.positionHCS);
                return o;
            }

            // フラグメントシェーダー
            half4 frag (Varyings i) : SV_Target
            {
                // スクリーン座標を正規化してUVに変換
                float2 screenUV = i.screenPos.xy / i.screenPos.w;

                // ボックスブラー処理
                half4 blurColor = 0;
                float step = _BlurSize / _BlurSamples;
                int samples = _BlurSamples;
                int sampleCount = 0;

                for (int x = -samples; x <= samples; x++)
                {
                    for (int y = -samples; y <= samples; y++)
                    {
                        float2 offset = float2(x * step, y * step);
                        // SAMPLE_TEXTURE2D_X_LODで背景テクスチャをサンプリング
                        blurColor += SAMPLE_TEXTURE2D_X_LOD(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + offset, 0);
                        sampleCount++;
                    }
                }
                blurColor /= sampleCount;

                // メインテクスチャの色を取得
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                texColor.a *= _TextureAlpha; // テクスチャの不透明度を調整

                // ぼかした背景色とテクスチャ色を、テクスチャのアルファ値で合成
                half4 finalColor = lerp(blurColor, texColor, texColor.a);

                // 全体の色合いとアルファ（不透明度）を適用
                finalColor.rgb *= _Color.rgb;
                finalColor.a = _Color.a;

                return finalColor;
            }
            ENDHLSL
        }
    }
}