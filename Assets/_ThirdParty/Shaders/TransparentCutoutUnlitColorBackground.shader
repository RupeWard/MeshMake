Shader "Custom/TransparentCutoutUnlitColorBackground" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Cutoff ("Cutoff", Range(0.01, 1)) = 0.25
	}
	SubShader 
	{
		Tags { "Queue" = "Transparent-1" }
		
		Pass
		{
			ZWrite Off
			Cull Off
         	Blend SrcAlpha OneMinusSrcAlpha
         	
			CGPROGRAM
			
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			uniform float4 _LightColor0;
			
	        sampler2D _MainTex;
	        float4 _MainTex_ST;
			uniform float4 _Color; 
			float _Cutoff;
			
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
	            fixed4 texColor = _Color * tex2D(_MainTex, i.uv.xy) * 2 * _LightColor0;
	            
	            //texColor.a = int(texColor.a / _Cutoff);
	            
	            if (texColor.a > _Cutoff)
	            	texColor.a = 1;
	            else
	            	texColor.a = 0;
	            
	            return texColor;
			}
			
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
