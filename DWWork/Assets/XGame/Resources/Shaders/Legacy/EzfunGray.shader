// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EZFun/EzfunGray" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Saturation("saturation",Range(0,1)) = 0.7
	}
	SubShader {
		Pass { 
		  ZTest Off Cull Off ZWrite Off
		  Fog { Mode off }
		  Blend Off

		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  #pragma fragmentoption ARB_precision_hint_fastest

		  #include "UnityCG.cginc"

		  uniform sampler2D _MainTex;
		  float _Saturation;

		  struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
		  };

		  v2f vert(appdata_img v) {
			v2f o;
			o.pos = UnityObjectToClipPos (v.vertex);
			o.uv = v.texcoord;
			return o;
		  }

		  half4 frag(v2f i) : COLOR {
			half4 col = tex2D(_MainTex, i.uv);
			half grey = dot(col.rgb, half3(0.299, 0.587, 0.114));
			grey = grey * _Saturation;
			half3 greyCol = half3(grey,grey,grey);
			col.rgb =  greyCol * _Saturation + (1 - _Saturation) * col.rgb;
			col.a = 1;
			return col;
		  }
		  ENDCG
		}
	}
}
