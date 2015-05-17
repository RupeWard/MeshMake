Shader "Custom/Unlit Color And Alpha" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Alpha ("Alpha", Float) = 0.5
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
			float _Alpha;
			
			struct inputData
			{
				float4 vertex : POSITION;
			};
			
			struct v2f
			{
				float4  pos : SV_POSITION;
			};

			v2f vert (inputData v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				return fixed4(_Color.rgb, _Alpha);
			}
			
			ENDCG
		}
	}
	
	FallBack "Diffuse"
}
