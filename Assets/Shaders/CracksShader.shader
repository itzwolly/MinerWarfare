Shader "Custom/CracksShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex ("Normal (RGB)", 2D) = "white" {}
		_CrackTex ("Cracks texture (RGB)", 2D) = "black" {}
		_Cracks ("Cracks", Range(0,1)) = 0.0
		_CrackColor ("CrackColor", Color) = (1,1,1,1)
		_CrackLimit ("Crack Limiter", Range(0,1)) = 0.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.0
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 500
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _CrackTex;
		sampler2D _NormalTex;
		//sampler2D _Glossiness;
		


		struct Input {
			float2 uv_MainTex;
			float2 uv_Normal;
		};

		half _Cracks;
		half _CrackLimit;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _CrackColor;
	
		


		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 cracksBaseColor = tex2D (_CrackTex, IN.uv_MainTex); //read pixel from snow



			half smoothRange=0;

			if (_CrackLimit <= 0 ){
				smoothRange=0.2;
				_Cracks*=(1+smoothRange);
			} else if (_CrackLimit >= 0) {
				//cracksBaseColor.r / c.rgb;
				smoothRange=0.2;
			}

			fixed3 col;

			if (cracksBaseColor.r >= _Cracks) {
				col = c.rgb;
			} else {
				
				col = _CrackColor; 
				//col = fixed3(1,1,1);//snow
			}

			if (col.r < _CrackLimit) col = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Albedo = col;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha =  c.a;
			o.Normal = UnpackNormal (tex2D (_NormalTex, IN.uv_MainTex) );

		}
		ENDCG
	}
	FallBack "Diffuse"
}
