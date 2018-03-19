// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:2,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1280277,fgcg:0.1953466,fgcb:0.2352941,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:4363,x:33341,y:32741,varname:node_4363,prsc:2|custl-5025-OUT,clip-4300-OUT;n:type:ShaderForge.SFN_Multiply,id:90,x:32666,y:32980,varname:node_90,prsc:2|A-7497-OUT,B-3868-RGB,C-7915-OUT;n:type:ShaderForge.SFN_Fresnel,id:7497,x:32430,y:32980,varname:node_7497,prsc:2|NRM-1689-OUT,EXP-6607-OUT;n:type:ShaderForge.SFN_NormalVector,id:1689,x:32162,y:33035,prsc:2,pt:False;n:type:ShaderForge.SFN_ValueProperty,id:6607,x:32147,y:33204,ptovrint:False,ptlb:exp,ptin:_exp,varname:node_6607,prsc:2,glob:False,v1:5;n:type:ShaderForge.SFN_Color,id:3868,x:32430,y:33190,ptovrint:False,ptlb:color,ptin:_color,varname:node_3868,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:7915,x:32430,y:33104,ptovrint:False,ptlb:fre,ptin:_fre,varname:node_7915,prsc:2,glob:False,v1:5;n:type:ShaderForge.SFN_Tex2d,id:1387,x:32148,y:32678,ptovrint:False,ptlb:node_1387,ptin:_node_1387,varname:node_1387,prsc:2,tex:7001614cd7f26464892706a460f024eb,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:5025,x:32760,y:32797,varname:node_5025,prsc:2|A-6933-OUT,B-90-OUT;n:type:ShaderForge.SFN_Multiply,id:6933,x:32589,y:32682,varname:node_6933,prsc:2|A-1387-RGB,B-5052-RGB,C-9810-OUT,D-1387-A;n:type:ShaderForge.SFN_Color,id:5052,x:32148,y:32852,ptovrint:False,ptlb:node_5052,ptin:_node_5052,varname:node_5052,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:9810,x:32426,y:32894,ptovrint:False,ptlb:diffuse_qd,ptin:_diffuse_qd,varname:node_9810,prsc:2,glob:False,v1:2;n:type:ShaderForge.SFN_Tex2d,id:2781,x:32892,y:33164,ptovrint:False,ptlb:aipha,ptin:_aipha,varname:node_2781,prsc:2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4300,x:33123,y:33033,varname:node_4300,prsc:2|A-1387-A,B-2781-A;proporder:6607-3868-7915-1387-5052-9810-2781;pass:END;sub:END;*/

Shader "Shader Forge/fer" {
    Properties {
        _exp ("exp", Float ) = 5
        _color ("color", Color) = (0.5,0.5,0.5,1)
        _fre ("fre", Float ) = 5
        _node_1387 ("node_1387", 2D) = "white" {}
        _node_5052 ("node_5052", Color) = (0.5,0.5,0.5,1)
        _diffuse_qd ("diffuse_qd", Float ) = 2
        _aipha ("aipha", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma target 3.0
            // Dithering function, to use with scene UVs (screen pixel coords)
            // 3x3 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
			half BinaryDither3x3(half value, half2 sceneUVs ) {
			half3x3 mtx = half3x3(
				half3( 3,  7,  4 )/10.0,
				half3( 6,  1,  9 )/10.0,
				half3( 2,  8,  5 )/10.0
                );
			half2 px = floor(_ScreenParams.xy * sceneUVs);
                int xSmp = fmod(px.x,3);
                int ySmp = fmod(px.y,3);
				half3 xVec = 1-saturate(abs(half3(0,1,2) - xSmp));
				half3 yVec = 1-saturate(abs(half3(0,1,2) - ySmp));
				half3 pxMult = half3( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec) );
                return round(value + dot(pxMult, xVec));
            }
            uniform float _exp;
            uniform float4 _color;
            uniform float _fre;
            uniform sampler2D _node_1387; uniform float4 _node_1387_ST;
            uniform float4 _node_5052;
            uniform float _diffuse_qd;
            uniform sampler2D _aipha; uniform float4 _aipha_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.pos;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
				half grabSign = -_ProjectionParams.x;
                #else
				half grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = half4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
				half2 sceneUVs = half2(1,grabSign)*i.screenPos.xy*0.5+0.5;
/////// Vectors:
				half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
				half3 normalDirection = i.normalDir;
				half4 _node_1387_var = tex2D(_node_1387,TRANSFORM_TEX(i.uv0, _node_1387));
				half4 _aipha_var = tex2D(_aipha,TRANSFORM_TEX(i.uv0, _aipha));
                clip( BinaryDither3x3((_node_1387_var.a*_aipha_var.a) - 1.5, sceneUVs) );
////// Lighting:
				half3 node_6933 = (_node_1387_var.rgb*_node_5052.rgb*_diffuse_qd*_node_1387_var.a);
				half node_7497 = pow(1.0-max(0,dot(i.normalDir, viewDirection)),_exp);
				half3 finalColor = (node_6933+(node_7497*_color.rgb*_fre));
                return fixed4(finalColor,1);
            }
            ENDCG
        }
//        Pass {
//            Name "ShadowCollector"
//            Tags {
//                "LightMode"="ShadowCollector"
//            }
//            
//            Fog {Mode Off}
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #define UNITY_PASS_SHADOWCOLLECTOR
//            #define SHADOW_COLLECTOR_PASS
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_shadowcollector
//            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
//            #pragma target 3.0
//            // Dithering function, to use with scene UVs (screen pixel coords)
//            // 3x3 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
//			half BinaryDither3x3(half value, half2 sceneUVs ) {
//			half3x3 mtx = half3x3(
//				half3( 3,  7,  4 )/10.0,
//				half3( 6,  1,  9 )/10.0,
//				half3( 2,  8,  5 )/10.0
//                );
//			half2 px = floor(_ScreenParams.xy * sceneUVs);
//                int xSmp = fmod(px.x,3);
//                int ySmp = fmod(px.y,3);
//				half3 xVec = 1-saturate(abs(half3(0,1,2) - xSmp));
//				half3 yVec = 1-saturate(abs(half3(0,1,2) - ySmp));
//				half3 pxMult = half3( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec) );
//                return round(value + dot(pxMult, xVec));
//            }
//            uniform sampler2D _node_1387; uniform float4 _node_1387_ST;
//            uniform sampler2D _aipha; uniform float4 _aipha_ST;
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float2 texcoord0 : TEXCOORD0;
//            };
//            struct VertexOutput {
//                V2F_SHADOW_COLLECTOR;
//                float2 uv0 : TEXCOORD5;
//                float4 screenPos : TEXCOORD6;
//            };
//            VertexOutput vert (VertexInput v) {
//                VertexOutput o = (VertexOutput)0;
//                o.uv0 = v.texcoord0;
//                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//                o.screenPos = o.pos;
//                TRANSFER_SHADOW_COLLECTOR(o)
//                return o;
//            }
//            fixed4 frag(VertexOutput i) : COLOR {
//                #if UNITY_UV_STARTS_AT_TOP
//				half grabSign = -_ProjectionParams.x;
//                #else
//				half grabSign = _ProjectionParams.x;
//                #endif
//                i.screenPos = half4( i.screenPos.xy / i.screenPos.w, 0, 0 );
//                i.screenPos.y *= _ProjectionParams.x;
//				half2 sceneUVs = half2(1,grabSign)*i.screenPos.xy*0.5+0.5;
///////// Vectors:
//				half4 _node_1387_var = tex2D(_node_1387,TRANSFORM_TEX(i.uv0, _node_1387));
//				half4 _aipha_var = tex2D(_aipha,TRANSFORM_TEX(i.uv0, _aipha));
//                clip( BinaryDither3x3((_node_1387_var.a*_aipha_var.a) - 1.5, sceneUVs) );
//                SHADOW_COLLECTOR_FRAGMENT(i)
//            }
//            ENDCG
//        }
//        Pass {
//            Name "ShadowCaster"
//            Tags {
//                "LightMode"="ShadowCaster"
//            }
//            Cull Off
//            Offset 1, 1
//            
//            Fog {Mode Off}
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #define UNITY_PASS_SHADOWCASTER
//            #include "UnityCG.cginc"
//            #include "Lighting.cginc"
//            #pragma fragmentoption ARB_precision_hint_fastest
//            #pragma multi_compile_shadowcaster
//            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
//            #pragma target 3.0
//            // Dithering function, to use with scene UVs (screen pixel coords)
//            // 3x3 Bayer matrix, based on https://en.wikipedia.org/wiki/Ordered_dithering
//					half BinaryDither3x3(half value, half2 sceneUVs ) {
//					half3x3 mtx = half3x3(
//						half3( 3,  7,  4 )/10.0,
//						half3( 6,  1,  9 )/10.0,
//						half3( 2,  8,  5 )/10.0
//                );
//					half2 px = floor(_ScreenParams.xy * sceneUVs);
//                int xSmp = fmod(px.x,3);
//                int ySmp = fmod(px.y,3);
//				half3 xVec = 1-saturate(abs(half3(0,1,2) - xSmp));
//				half3 yVec = 1-saturate(abs(half3(0,1,2) - ySmp));
//				half3 pxMult = half3( dot(mtx[0],yVec), dot(mtx[1],yVec), dot(mtx[2],yVec) );
//                return round(value + dot(pxMult, xVec));
//            }
//            uniform sampler2D _node_1387; uniform float4 _node_1387_ST;
//            uniform sampler2D _aipha; uniform float4 _aipha_ST;
//            struct VertexInput {
//                float4 vertex : POSITION;
//                float2 texcoord0 : TEXCOORD0;
//            };
//            struct VertexOutput {
//                V2F_SHADOW_CASTER;
//                float2 uv0 : TEXCOORD1;
//                float4 screenPos : TEXCOORD2;
//            };
//            VertexOutput vert (VertexInput v) {
//                VertexOutput o = (VertexOutput)0;
//                o.uv0 = v.texcoord0;
//                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
//                o.screenPos = o.pos;
//                TRANSFER_SHADOW_CASTER(o)
//                return o;
//            }
//            fixed4 frag(VertexOutput i) : COLOR {
//                #if UNITY_UV_STARTS_AT_TOP
//				half grabSign = -_ProjectionParams.x;
//                #else
//				half grabSign = _ProjectionParams.x;
//                #endif
//                i.screenPos = half4( i.screenPos.xy / i.screenPos.w, 0, 0 );
//                i.screenPos.y *= _ProjectionParams.x;
//				half2 sceneUVs = half2(1,grabSign)*i.screenPos.xy*0.5+0.5;
///////// Vectors:
//				half4 _node_1387_var = tex2D(_node_1387,TRANSFORM_TEX(i.uv0, _node_1387));
//				half4 _aipha_var = tex2D(_aipha,TRANSFORM_TEX(i.uv0, _aipha));
//                clip( BinaryDither3x3((_node_1387_var.a*_aipha_var.a) - 1.5, sceneUVs) );
//                SHADOW_CASTER_FRAGMENT(i)
//            }
//            ENDCG
//        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
