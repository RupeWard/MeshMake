Shader "Custom/SkyDomeTransparentOverlay" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color Tint", Color) = (1,1,1,1)
		_Alpha ("Alpha", Range(0, 1)) = 1
	}
	SubShader 
	{
		Tags { "Queue" = "Transparent" }
		
		Pass
		{
			ZWrite On
         	Blend SrcAlpha One
         	
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform float4 _Color;
			float _Alpha;
			
			struct output
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};
			
			output vert(appdata_full v)
			{
				output o;
				
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	            
				return o;
			}
			
			float4 frag(output o) : COLOR
			{
				float4 col = _Color * tex2D(_MainTex, o.uv.xy);
				col.a = col.a * _Alpha;
				return col;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
