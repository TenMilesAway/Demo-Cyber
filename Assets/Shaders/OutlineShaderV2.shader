Shader "Unlit/OutlineV2"
{
    Properties
    {
        _OutlineWidth ("Outline Width", Range(0.001, 0.1)) = 0.02
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _MainTexColor ("MainTex Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "IgnoreProjector"="True"
        }

        // 基础渲染Pass
        Pass
        {
            Name "Main"
            Tags { "LightMode"="UniversalForward" }
            
            Cull Back
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _MainTexColor;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = positionInputs.positionCS;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _MainTexColor;
            }
            ENDHLSL
        }

        // 轮廓线Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" } // 使用无光照模式
            
            Cull Front
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
                float _OutlineWidth;
                half4 _OutlineColor;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // 获取顶点位置输入
                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                
                // 获取法线（需要转换到视空间）
                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS);
                
                // 在视空间进行外扩
                float3 viewPos = TransformWorldToView(positionInputs.positionWS);
                float3 viewNormal = TransformWorldToViewDir(normalInputs.normalWS);
                viewNormal = normalize(viewNormal);
                
                // 在视空间外扩
                viewPos += viewNormal * _OutlineWidth;
                
                // 转换回裁剪空间
                OUT.positionHCS = TransformWViewToHClip(viewPos);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}