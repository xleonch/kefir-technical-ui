Shader "ui_add_generated"
{
    Properties
    {
        [NoScaleOffset]_MainTex("_MainTex", 2D) = "white" {}
        [HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector]_ColorMask("ColorMask", Float) = 15
        [HideInInspector]_ClipRect("ClipRect", Vector, 4) = (0, 0, 0, 0)
        [HideInInspector]_UIMaskSoftnessX("UIMaskSoftnessX", Float) = 1
        [HideInInspector]_UIMaskSoftnessY("UIMaskSoftnessY", Float) = 1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalCanvasSubTarget"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Pass
        {
            Name "Default"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Off
        Blend One One
        ZTest [unity_GUIZTestMode]
        ZWrite Off
        ColorMask [_ColorMask]
        Stencil
        {
        ReadMask [_StencilReadMask]
        WriteMask [_StencilWriteMask]
        Ref [_Stencil]
        CompFront [_StencilComp]
        PassFront [_StencilOp]
        CompBack [_StencilComp]
        PassBack [_StencilOp]
        }

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag

            // Keywords
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
        #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            // GraphKeywords: <None>

            #define CANVAS_SHADERGRAPH

            // Defines
           #define _SURFACE_TYPE_TRANSPARENT 1
           #define ATTRIBUTES_NEED_NORMAL
           #define ATTRIBUTES_NEED_TEXCOORD0
           #define ATTRIBUTES_NEED_TEXCOORD1
           #define ATTRIBUTES_NEED_COLOR
           #define ATTRIBUTES_NEED_VERTEXID
           #define ATTRIBUTES_NEED_INSTANCEID
           #define VARYINGS_NEED_POSITION_WS
           #define VARYINGS_NEED_NORMAL_WS
           #define VARYINGS_NEED_TEXCOORD0
           #define VARYINGS_NEED_TEXCOORD1
           #define VARYINGS_NEED_COLOR

        #define REQUIRE_DEPTH_TEXTURE
        #define REQUIRE_NORMAL_TEXTURE

           #define SHADERPASS SHADERPASS_CUSTOM_UI

           #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/DebugMipmapStreamingMacros.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
        #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

            // --------------------------------------------------
            // Structs and Packing


            struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 color : COLOR;
             float4 uv0 : TEXCOORD0;
             float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(ATTRIBUTES_NEED_INSTANCEID)
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
             uint vertexID : VERTEXID_SEMANTIC;
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float4 VertexColor;
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
             float4 texCoord1;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };
        struct VertexDescriptionInputs
        {
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 texCoord1 : INTERP1;
             float4 color : INTERP2;
             float3 positionWS : INTERP3;
             float3 normalWS : INTERP4;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.texCoord1.xyzw = input.texCoord1;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }

        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.texCoord1 = input.texCoord1.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED || defined(VARYINGS_NEED_INSTANCEID)
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            return output;
        }


            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            //UGUI has no keyword for when a renderer has "bloom", so its nessecary to hardcore it here, like all the base UI shaders.
            half4 _TextureSampleAdd;

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Stencil;
        float _StencilOp;
        float _StencilWriteMask;
        float _StencilReadMask;
        float _ColorMask;
        float4 _ClipRect;
        float _UIMaskSoftnessX;
        float _UIMaskSoftnessY;
        UNITY_TEXTURE_STREAMING_DEBUG_VARS;
        CBUFFER_END


        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);

            // Graph Includes
            // GraphIncludes: <None>

            // Graph Functions

        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }

            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            // GraphVertex: <None>

            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreSurface' */

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float3 Emission;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_c7633a060c42480191e38587e5f345a3_Out_0_Texture2D = UnityBuildTexture2DStructInternal(_MainTex, sampler_MainTex, _MainTex_TexelSize, float4(1, 1, 0, 0), float4(0, 0, 0, 0));
            float4 _SampleTexture2D_0869842ff4df4c06b916327adb31679e_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_c7633a060c42480191e38587e5f345a3_Out_0_Texture2D.tex, _Property_c7633a060c42480191e38587e5f345a3_Out_0_Texture2D.samplerstate, _Property_c7633a060c42480191e38587e5f345a3_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            if (_Property_c7633a060c42480191e38587e5f345a3_Out_0_Texture2D.hdrDecode.x > 0)
                _SampleTexture2D_0869842ff4df4c06b916327adb31679e_RGBA_0_Vector4 = DecodeHDRSample(_SampleTexture2D_0869842ff4df4c06b916327adb31679e_RGBA_0_Vector4, _Property_c7633a060c42480191e38587e5f345a3_Out_0_Texture2D.hdrDecode);
            float _SampleTexture2D_0869842ff4df4c06b916327adb31679e_R_4_Float = _SampleTexture2D_0869842ff4df4c06b916327adb31679e_RGBA_0_Vector4.r;
            float _SampleTexture2D_0869842ff4df4c06b916327adb31679e_G_5_Float = _SampleTexture2D_0869842ff4df4c06b916327adb31679e_RGBA_0_Vector4.g;
            float _SampleTexture2D_0869842ff4df4c06b916327adb31679e_B_6_Float = _SampleTexture2D_0869842ff4df4c06b916327adb31679e_RGBA_0_Vector4.b;
            float _SampleTexture2D_0869842ff4df4c06b916327adb31679e_A_7_Float = _SampleTexture2D_0869842ff4df4c06b916327adb31679e_RGBA_0_Vector4.a;
            float4 _Multiply_5c5cec6316c342d1800becc25e013e6a_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_0869842ff4df4c06b916327adb31679e_RGBA_0_Vector4, IN.VertexColor, _Multiply_5c5cec6316c342d1800becc25e013e6a_Out_2_Vector4);
            float _Split_d91a62225beb430ebe6e33b668814e3c_R_1_Float = _Multiply_5c5cec6316c342d1800becc25e013e6a_Out_2_Vector4[0];
            float _Split_d91a62225beb430ebe6e33b668814e3c_G_2_Float = _Multiply_5c5cec6316c342d1800becc25e013e6a_Out_2_Vector4[1];
            float _Split_d91a62225beb430ebe6e33b668814e3c_B_3_Float = _Multiply_5c5cec6316c342d1800becc25e013e6a_Out_2_Vector4[2];
            float _Split_d91a62225beb430ebe6e33b668814e3c_A_4_Float = _Multiply_5c5cec6316c342d1800becc25e013e6a_Out_2_Vector4[3];
            surface.BaseColor = (_Multiply_5c5cec6316c342d1800becc25e013e6a_Out_2_Vector4.xyz);
            surface.Alpha = _Split_d91a62225beb430ebe6e33b668814e3c_A_4_Float;
            surface.Emission = float3(0, 0, 0);
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorCopyToSDI' */






            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif



        #if defined(UNITY_UIE_INCLUDED)
            output.uv0 =                                        float4(input.texCoord0.x, input.texCoord0.y, 0, 0);
        #else
            output.uv0 =                                        input.texCoord0;
        #endif

            output.VertexColor =                                input.color;



        #if UNITY_ANY_INSTANCING_ENABLED
        #else // TODO: XR support for procedural instancing because in this case UNITY_ANY_INSTANCING_ENABLED is not defined and instanceID is incorrect.
        #endif
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/CanvasPass.hlsl"

            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}