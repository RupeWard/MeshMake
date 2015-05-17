Shader "Custom/UnlitTextureColor" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color Tint", Color) = (1,1,1,1)
	}
	SubShader 
	{
		Pass
		{			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			//Textures
			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform float4 _Color;
			
			struct v2f
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};

			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				return _Color * tex2D(_MainTex, i.uv.xy);
			}
			ENDCG
		}
	} 
	//FallBack "Diffuse"
}
