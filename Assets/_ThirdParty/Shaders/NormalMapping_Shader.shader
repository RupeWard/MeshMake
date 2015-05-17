Shader "Custom/NormalMapping"
{
	Properties
	{
		_MainTex ("Main", 2D) = "white" {}
		_NormalTex ("Normal Map", 2D) = "bump" {}
	}
	SubShader
	{	
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _NormalTex;

		struct Input
		{
			float2 uv_NormalTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			//float3 normalMap = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex));
			
			//o.Normal = normalMap.rgb;
		}
		
		ENDCG
	} 
	FallBack "Diffuse"
}
