Shader "Stick Sports/Diffuse Vertex Color" 
{
	Properties 
	{
		
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		struct Input 
		{
			fixed4 vertColor;
		};
		
		void vert(inout appdata_full v, out Input o)
		{
			o.vertColor = v.color;
		}

		void surf (Input IN, inout SurfaceOutput o) 
		{
			o.Albedo = IN.vertColor.rgb;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
