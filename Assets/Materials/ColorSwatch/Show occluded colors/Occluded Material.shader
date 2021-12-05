Shader "Color"
{
    Properties
    {
        _Color("Color", Color) = (0.9882353, 0.3215686, 0.3960784, 1)
        Vector1_4D553003("Occuluded Threshold", Float) = 0.1
        Vector1_05be4ba04b734995aae6b052b1f49be1("Occlusion Transparent Amount", Float) = 0.1
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
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Pass"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest always
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
        #define REQUIRE_DEPTH_TEXTURE
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 ViewSpacePosition;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _Color;
        float Vector1_4D553003;
        float Vector1_05be4ba04b734995aae6b052b1f49be1;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Comparison_LessOrEqual_float(float A, float B, out float Out)
        {
            Out = A <= B ? 1 : 0;
        }

        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_R_1 = IN.ViewSpacePosition[0];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_G_2 = IN.ViewSpacePosition[1];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_B_3 = IN.ViewSpacePosition[2];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_A_4 = 0;
            float _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1;
            Unity_Negate_float(_Split_4ae40589b9da6388975c2c2a6034f4a1_B_3, _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1);
            float _Property_f869b90ecc027e8f8380993a7d13df02_Out_0 = Vector1_4D553003;
            float _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2;
            Unity_Subtract_float(_Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1, _Property_f869b90ecc027e8f8380993a7d13df02_Out_0, _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2);
            float4 _ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1;
            Unity_SceneDepth_Eye_float(_ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1);
            float _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2;
            Unity_Comparison_LessOrEqual_float(_Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1, _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2);
            float4 _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0 = _Color;
            float _Property_df84584ee0dc43c190cc09532f52cd15_Out_0 = Vector1_05be4ba04b734995aae6b052b1f49be1;
            float4 _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0 = float4(0, 0, 0, _Property_df84584ee0dc43c190cc09532f52cd15_Out_0);
            float4 _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2;
            Unity_Subtract_float4(_Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2);
            float4 _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3;
            Unity_Branch_float4(_Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2, _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2, _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3);
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_R_1 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[0];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_G_2 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[1];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_B_3 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[2];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[3];
            surface.BaseColor = (_Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3.xyz);
            surface.Alpha = _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ViewSpacePosition =           TransformWorldToView(input.positionWS);
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
        #define REQUIRE_DEPTH_TEXTURE
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 ViewSpacePosition;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _Color;
        float Vector1_4D553003;
        float Vector1_05be4ba04b734995aae6b052b1f49be1;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Comparison_LessOrEqual_float(float A, float B, out float Out)
        {
            Out = A <= B ? 1 : 0;
        }

        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_R_1 = IN.ViewSpacePosition[0];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_G_2 = IN.ViewSpacePosition[1];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_B_3 = IN.ViewSpacePosition[2];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_A_4 = 0;
            float _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1;
            Unity_Negate_float(_Split_4ae40589b9da6388975c2c2a6034f4a1_B_3, _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1);
            float _Property_f869b90ecc027e8f8380993a7d13df02_Out_0 = Vector1_4D553003;
            float _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2;
            Unity_Subtract_float(_Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1, _Property_f869b90ecc027e8f8380993a7d13df02_Out_0, _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2);
            float4 _ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1;
            Unity_SceneDepth_Eye_float(_ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1);
            float _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2;
            Unity_Comparison_LessOrEqual_float(_Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1, _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2);
            float4 _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0 = _Color;
            float _Property_df84584ee0dc43c190cc09532f52cd15_Out_0 = Vector1_05be4ba04b734995aae6b052b1f49be1;
            float4 _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0 = float4(0, 0, 0, _Property_df84584ee0dc43c190cc09532f52cd15_Out_0);
            float4 _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2;
            Unity_Subtract_float4(_Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2);
            float4 _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3;
            Unity_Branch_float4(_Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2, _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2, _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3);
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_R_1 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[0];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_G_2 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[1];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_B_3 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[2];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[3];
            surface.Alpha = _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ViewSpacePosition =           TransformWorldToView(input.positionWS);
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define REQUIRE_DEPTH_TEXTURE
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 ViewSpacePosition;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _Color;
        float Vector1_4D553003;
        float Vector1_05be4ba04b734995aae6b052b1f49be1;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Comparison_LessOrEqual_float(float A, float B, out float Out)
        {
            Out = A <= B ? 1 : 0;
        }

        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_R_1 = IN.ViewSpacePosition[0];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_G_2 = IN.ViewSpacePosition[1];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_B_3 = IN.ViewSpacePosition[2];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_A_4 = 0;
            float _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1;
            Unity_Negate_float(_Split_4ae40589b9da6388975c2c2a6034f4a1_B_3, _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1);
            float _Property_f869b90ecc027e8f8380993a7d13df02_Out_0 = Vector1_4D553003;
            float _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2;
            Unity_Subtract_float(_Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1, _Property_f869b90ecc027e8f8380993a7d13df02_Out_0, _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2);
            float4 _ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1;
            Unity_SceneDepth_Eye_float(_ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1);
            float _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2;
            Unity_Comparison_LessOrEqual_float(_Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1, _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2);
            float4 _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0 = _Color;
            float _Property_df84584ee0dc43c190cc09532f52cd15_Out_0 = Vector1_05be4ba04b734995aae6b052b1f49be1;
            float4 _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0 = float4(0, 0, 0, _Property_df84584ee0dc43c190cc09532f52cd15_Out_0);
            float4 _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2;
            Unity_Subtract_float4(_Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2);
            float4 _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3;
            Unity_Branch_float4(_Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2, _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2, _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3);
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_R_1 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[0];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_G_2 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[1];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_B_3 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[2];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[3];
            surface.Alpha = _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ViewSpacePosition =           TransformWorldToView(input.positionWS);
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
        }
        Pass
        {
            Name "Pass"
            Tags
            {
                // LightMode: <None>
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_UNLIT
        #define REQUIRE_DEPTH_TEXTURE
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 ViewSpacePosition;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _Color;
        float Vector1_4D553003;
        float Vector1_05be4ba04b734995aae6b052b1f49be1;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Comparison_LessOrEqual_float(float A, float B, out float Out)
        {
            Out = A <= B ? 1 : 0;
        }

        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_R_1 = IN.ViewSpacePosition[0];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_G_2 = IN.ViewSpacePosition[1];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_B_3 = IN.ViewSpacePosition[2];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_A_4 = 0;
            float _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1;
            Unity_Negate_float(_Split_4ae40589b9da6388975c2c2a6034f4a1_B_3, _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1);
            float _Property_f869b90ecc027e8f8380993a7d13df02_Out_0 = Vector1_4D553003;
            float _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2;
            Unity_Subtract_float(_Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1, _Property_f869b90ecc027e8f8380993a7d13df02_Out_0, _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2);
            float4 _ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1;
            Unity_SceneDepth_Eye_float(_ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1);
            float _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2;
            Unity_Comparison_LessOrEqual_float(_Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1, _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2);
            float4 _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0 = _Color;
            float _Property_df84584ee0dc43c190cc09532f52cd15_Out_0 = Vector1_05be4ba04b734995aae6b052b1f49be1;
            float4 _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0 = float4(0, 0, 0, _Property_df84584ee0dc43c190cc09532f52cd15_Out_0);
            float4 _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2;
            Unity_Subtract_float4(_Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2);
            float4 _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3;
            Unity_Branch_float4(_Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2, _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2, _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3);
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_R_1 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[0];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_G_2 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[1];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_B_3 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[2];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[3];
            surface.BaseColor = (_Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3.xyz);
            surface.Alpha = _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ViewSpacePosition =           TransformWorldToView(input.positionWS);
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_SHADOWCASTER
        #define REQUIRE_DEPTH_TEXTURE
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 ViewSpacePosition;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _Color;
        float Vector1_4D553003;
        float Vector1_05be4ba04b734995aae6b052b1f49be1;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Comparison_LessOrEqual_float(float A, float B, out float Out)
        {
            Out = A <= B ? 1 : 0;
        }

        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_R_1 = IN.ViewSpacePosition[0];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_G_2 = IN.ViewSpacePosition[1];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_B_3 = IN.ViewSpacePosition[2];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_A_4 = 0;
            float _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1;
            Unity_Negate_float(_Split_4ae40589b9da6388975c2c2a6034f4a1_B_3, _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1);
            float _Property_f869b90ecc027e8f8380993a7d13df02_Out_0 = Vector1_4D553003;
            float _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2;
            Unity_Subtract_float(_Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1, _Property_f869b90ecc027e8f8380993a7d13df02_Out_0, _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2);
            float4 _ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1;
            Unity_SceneDepth_Eye_float(_ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1);
            float _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2;
            Unity_Comparison_LessOrEqual_float(_Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1, _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2);
            float4 _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0 = _Color;
            float _Property_df84584ee0dc43c190cc09532f52cd15_Out_0 = Vector1_05be4ba04b734995aae6b052b1f49be1;
            float4 _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0 = float4(0, 0, 0, _Property_df84584ee0dc43c190cc09532f52cd15_Out_0);
            float4 _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2;
            Unity_Subtract_float4(_Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2);
            float4 _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3;
            Unity_Branch_float4(_Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2, _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2, _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3);
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_R_1 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[0];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_G_2 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[1];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_B_3 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[2];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[3];
            surface.Alpha = _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ViewSpacePosition =           TransformWorldToView(input.positionWS);
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"

            ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            // Render State
            Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        ColorMask 0

            // Debug
            // <None>

            // --------------------------------------------------
            // Pass

            HLSLPROGRAM

            // Pragmas
            #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag

            // DotsInstancingOptions: <None>
            // HybridV1InjectedBuiltinProperties: <None>

            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>

            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS
            #define FEATURES_GRAPH_VERTEX
            /* WARNING: $splice Could not find named fragment 'PassInstancing' */
            #define SHADERPASS SHADERPASS_DEPTHONLY
        #define REQUIRE_DEPTH_TEXTURE
            /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

            // --------------------------------------------------
            // Structs and Packing

            struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
            float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
            float4 positionCS : SV_POSITION;
            float3 positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
            float3 ViewSpacePosition;
            float3 WorldSpacePosition;
            float4 ScreenPosition;
        };
        struct VertexDescriptionInputs
        {
            float3 ObjectSpaceNormal;
            float3 ObjectSpaceTangent;
            float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
            float4 positionCS : SV_POSITION;
            float3 interp0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
            uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

            PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

            // --------------------------------------------------
            // Graph

            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
        float4 _Color;
        float Vector1_4D553003;
        float Vector1_05be4ba04b734995aae6b052b1f49be1;
        CBUFFER_END

        // Object and Global properties

            // Graph Functions
            
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }

        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }

        void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
        {
            Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
        }

        void Unity_Comparison_LessOrEqual_float(float A, float B, out float Out)
        {
            Out = A <= B ? 1 : 0;
        }

        void Unity_Subtract_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A - B;
        }

        void Unity_Branch_float4(float Predicate, float4 True, float4 False, out float4 Out)
        {
            Out = Predicate ? True : False;
        }

            // Graph Vertex
            struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };

        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }

            // Graph Pixel
            struct SurfaceDescription
        {
            float Alpha;
        };

        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_R_1 = IN.ViewSpacePosition[0];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_G_2 = IN.ViewSpacePosition[1];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_B_3 = IN.ViewSpacePosition[2];
            float _Split_4ae40589b9da6388975c2c2a6034f4a1_A_4 = 0;
            float _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1;
            Unity_Negate_float(_Split_4ae40589b9da6388975c2c2a6034f4a1_B_3, _Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1);
            float _Property_f869b90ecc027e8f8380993a7d13df02_Out_0 = Vector1_4D553003;
            float _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2;
            Unity_Subtract_float(_Negate_01c65901d860928dab08a9cf6fea7e5d_Out_1, _Property_f869b90ecc027e8f8380993a7d13df02_Out_0, _Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2);
            float4 _ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0 = float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0);
            float _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1;
            Unity_SceneDepth_Eye_float(_ScreenPosition_703817bef4c041b0b11b0ea49ab95748_Out_0, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1);
            float _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2;
            Unity_Comparison_LessOrEqual_float(_Subtract_922627ccffaf5580a4f21edfc1d9c2f7_Out_2, _SceneDepth_d93ec6dae652fd8f8988461501aa2c61_Out_1, _Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2);
            float4 _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0 = _Color;
            float _Property_df84584ee0dc43c190cc09532f52cd15_Out_0 = Vector1_05be4ba04b734995aae6b052b1f49be1;
            float4 _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0 = float4(0, 0, 0, _Property_df84584ee0dc43c190cc09532f52cd15_Out_0);
            float4 _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2;
            Unity_Subtract_float4(_Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Vector4_f0e64a64cfe2402bb0efed35b8be93d6_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2);
            float4 _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3;
            Unity_Branch_float4(_Comparison_3f24f3a3aca2ea8985b644c86e5308df_Out_2, _Property_84951333259bca81a1a1c676a7ae1f3c_Out_0, _Subtract_070d4aabf2b54c908407fb6d5a5f3758_Out_2, _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3);
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_R_1 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[0];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_G_2 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[1];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_B_3 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[2];
            float _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4 = _Branch_0fcbaff20bf4148592cd2246bd79ef99_Out_3[3];
            surface.Alpha = _Split_cce0076bdc9745edbef1ae3ae0c1a4b9_A_4;
            return surface;
        }

            // --------------------------------------------------
            // Build Graph Inputs

            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);

            output.ObjectSpaceNormal =           input.normalOS;
            output.ObjectSpaceTangent =          input.tangentOS.xyz;
            output.ObjectSpacePosition =         input.positionOS;

            return output;
        }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);





            output.WorldSpacePosition =          input.positionWS;
            output.ViewSpacePosition =           TransformWorldToView(input.positionWS);
            output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

            return output;
        }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"

            ENDHLSL
        }
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}