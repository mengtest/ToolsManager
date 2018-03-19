// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:True;n:type:ShaderForge.SFN_Final,id:9081,x:32965,y:32650,varname:node_9081,prsc:2|emission-7394-OUT,alpha-9618-OUT;n:type:ShaderForge.SFN_Tex2d,id:1620,x:31968,y:32716,ptovrint:False,ptlb:t_贴图,ptin:_t_,varname:node_1620,prsc:2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9618,x:32533,y:32869,varname:node_9618,prsc:2|A-1620-R,B-2967-OUT,C-1620-A;n:type:ShaderForge.SFN_Fresnel,id:729,x:32031,y:32907,varname:node_729,prsc:2|EXP-1063-OUT;n:type:ShaderForge.SFN_Multiply,id:2967,x:32287,y:32944,varname:node_2967,prsc:2|A-729-OUT,B-2549-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2549,x:32015,y:33119,ptovrint:False,ptlb:fer—边缘控制,ptin:_fer,varname:_03,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:7394,x:32773,y:32744,varname:node_7394,prsc:2|A-5306-OUT,B-9618-OUT;n:type:ShaderForge.SFN_Color,id:9458,x:32001,y:32515,ptovrint:False,ptlb:b颜色,ptin:_b,varname:node_9458,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:1769,x:32219,y:32693,ptovrint:False,ptlb:a颜色强度,ptin:_a,varname:node_1769,prsc:2,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:5306,x:32560,y:32662,varname:node_5306,prsc:2|A-9458-RGB,B-1769-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1063,x:31803,y:32923,ptovrint:False,ptlb:rfe—中心,ptin:_rfe,varname:node_1063,prsc:2,glob:False,v1:0.01;proporder:1620-2549-9458-1769-1063;pass:END;sub:END;*/

Shader "Shader Forge/fer_add" {
    Properties {
        _t_ ("t_贴图", 2D) = "white" {}
        _fer ("fer—边缘控制", Float ) = 1
        _b ("b颜色", Color) = (0.5,0.5,0.5,1)
        _a ("a颜色强度", Float ) = 1
        _rfe ("rfe—中心", Float ) = 0.01
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            //#pragma multi_compile_fwdbase
            uniform sampler2D _t_; uniform float4 _t__ST;
            uniform float _fer;
            uniform float4 _b;
            uniform float _a;
            uniform float _rfe;
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
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
			half3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
			half3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
			half4 _t__var = tex2D(_t_,i.uv0);
			half node_729 = pow(1.0-max(0,dot(normalDirection, viewDirection)),_rfe);
			half node_9618 = (_t__var.r*(node_729*_fer)*_t__var.a);
			half3 emissive = ((_b.rgb*_a)*node_9618);
			half3 finalColor = emissive;
                return fixed4(finalColor,node_9618);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
