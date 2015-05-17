Shader "Custom/AlphaColour" 
{
	Properties
   	{
      	_Color ("Main Color", Color) = (1,1,1,1)
   	}
   	
	SubShader
	{
		Tags { "Queue" = "Transparent" }
	
		Pass
		{
			ZWrite Off
         	Blend SrcAlpha OneMinusSrcAlpha 
         
			CGPROGRAM
			
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			uniform float4 _Color; 
			
			struct v2f
			{
				float4  pos : SV_POSITION;
			};
	
			v2f vert (appdata_full v)
			{
				v2f o;
				
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	            
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
	            fixed4 texColor = _Color;
	            
				return texColor;
			}
			
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}