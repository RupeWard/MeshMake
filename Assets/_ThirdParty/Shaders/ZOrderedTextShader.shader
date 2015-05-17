Shader "Custom/ZOrderedTextShader" 
{
	Properties 
	{
	   	_MainTex ("Font Texture", 2D) = "white" {} 
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader 
	{
		Tags
   		{
   			"Queue" 			= "Transparent"
   			"IgnoreProjector" 	= "True"
   			"RenderType" 		= "Transparent"
   		}
   		
		Pass
		{   				
			Lighting Off ZWrite Off Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
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
				fixed4 texColor = tex2D(_MainTex, i.uv.xy);
				texColor.rgb = _Color.rgb;
				return texColor;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
