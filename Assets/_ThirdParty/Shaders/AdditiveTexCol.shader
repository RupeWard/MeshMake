Shader "Custom/AdditiveTexCol" 
{
	Properties 
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color tint", Color) = (1,1,1,1)
	}
	SubShader 
	{
		Tags { "Queue" = "Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Blend SrcAlpha One
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers xbox360 ps3 flash
			
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
	            //fixed4 texColor = _Color * tex2D(_MainTex, i.uv.xy);            
				return _Color * tex2D(_MainTex, i.uv.xy);
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
