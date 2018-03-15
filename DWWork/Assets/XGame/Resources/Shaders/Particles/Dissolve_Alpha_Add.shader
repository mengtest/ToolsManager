// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EZFun/Dissolve_Alpha_add" {
	Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Tile("Tile",float) = 1
	_DissColor("DissColor",Color) = (1,1,1,1)
	_Amount ("Amount",Range(0,1)) = 0.5
	_StartAmount ("StartAmount",float) = 0.1
	_DissolveSrc("DissolveSrc",2D) = "white" {}
}

Category {
	Tags {"Queue"="Transparent" "RenderType"="Transparent"}
	Blend SrcAlpha One
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off

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
			static half3 Color = float3(1,1,1);
			half _Tile;
			half _StartAmount;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD1;
				#endif
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o = (v2f)0;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
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
						Clip = 1; //clip(-0.1);
					
					}
					 else
					 {
					
						if (ClipAmount < _StartAmount)
						{
							if (_ColorAnimate.x == 0)
								Color.x += _DissColor.x ;
							else
								Color.x += ClipAmount/_StartAmount ;
				          
							if (_ColorAnimate.y == 0)
								Color.y += _DissColor.y ;
							else
								Color.y += ClipAmount/_StartAmount;
				          
							if (_ColorAnimate.z == 0)
								Color.z += _DissColor.z ;
							else
								Color.z += ClipAmount/_StartAmount;

						//	o.Albedo  = (o.Albedo *((Color.x+Color.y+Color.z))* Color*((Color.x+Color.y+Color.z)))/(1 - _Illuminate);
						//	o.Normal = UnpackNormal(tex2D(_DissolveSrcBump, IN.uvDissolveSrc));
						
						}
					 }
				 }

 
				if (Clip == 1)
				{
				clip(-0.1);
				}
				
				fixed3 c = Color.xyz * Color.xyz ;
				//i.color.rgb = c + c;
				fixed4 tex = 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
				tex.rgb = tex.rgb * c;
				return tex ;
			}
			ENDCG 
		}
	}	
	}
}
