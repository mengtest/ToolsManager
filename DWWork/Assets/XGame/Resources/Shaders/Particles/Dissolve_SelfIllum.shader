// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EZFun/Dissolve_SelfIllum" {
	Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Tile("Tile",float) = 1
	_DissColor("DissColor",Color) = (1,1,1,1)
	_Amount ("Amount",Range(0,1)) = 0.5
	_StartAmount ("StartAmount",float) = 0.1
	_DissolveSrc("DissolveSrc",2D) = "white" {}
	_StaticColor("StaticColor",Color) = (1.0,1.0,1.0,1.0)
}

Category {
	Tags {"RenderType"="Opaque"}
	 Cull Off
	 Lighting Off 

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			sampler2D _DissolveSrc;
			half4 _DissColor;
			half _Amount;
			half4 _ColorAnimate;
			half _Tile;
			half _StartAmount;
			fixed4 _StaticColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				half ClipTex = tex2D (_DissolveSrc, i.texcoord/_Tile).r ;
			half ClipAmount = ClipTex - _Amount;
			half Clip = 0;
				if (_Amount > 0)
				{
					if (ClipAmount <0)
					{
						Clip = 1; 
					}
					else
					{
						if (ClipAmount < _StartAmount)
						{
							if (_ColorAnimate.x == 0)
								_StaticColor.x += _DissColor.x ;
							else
								_StaticColor.x += ClipAmount/_StartAmount ;
				          
							if (_ColorAnimate.y == 0)
								_StaticColor.y += _DissColor.y ;
							else
								_StaticColor.y += ClipAmount/_StartAmount;
				          
							if (_ColorAnimate.z == 0)
								_StaticColor.z += _DissColor.z ;
							else
								_StaticColor.z += ClipAmount/_StartAmount;
						}
					}
				}
 
				if (Clip == 1)
				{
					clip(-0.1);
				}
				
				fixed3 c = _StaticColor.xyz * _StaticColor.xyz ;
				fixed4 tex = 2.0f * _TintColor * tex2D(_MainTex, i.texcoord);
				tex.rgb = tex.rgb * c;
				return tex ;
			}
			ENDCG 
		}
	}	
	}
}
