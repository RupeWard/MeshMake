Shader "Stick Sports/Diffuse Colour" {
	Properties
	{
		_MainTint("Global Color Tint", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert addshadow

		float4 _MainTint;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = _MainTint.rgb;
		}
		ENDCG
	} 
	FallBack "Mobile/Diffuse"
}
