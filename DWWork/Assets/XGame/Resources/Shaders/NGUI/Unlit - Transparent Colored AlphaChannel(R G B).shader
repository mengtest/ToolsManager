// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Transparent Colored AlphaChannel(R(X) G(Y) B(Z))"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "black" {}
		_AlphaTex("Alpha (R)", 2D) = "white"{}
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			sampler2D _MainTex;
			sampler2D _AlphaTex;
				
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 color = fixed4(tex2D(_MainTex, i.texcoord).rgb, tex2D(_AlphaTex, i.texcoord).r);
			
				half gray = dot(color.rgb, half3(0.299, 0.587, 0.114));
				half hasColor = dot(i.color.rgb, half3(1, 1, 1));
				color.rgb = hasColor == 0 ? gray.xxx : color.rgb * i.color.rgb;
				color.a *= i.color.a;
			    return color;
			}
			ENDCG
		}
	}

}
