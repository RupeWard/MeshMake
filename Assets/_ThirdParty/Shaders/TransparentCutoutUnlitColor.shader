Shader "Custom/TransparentCutoutUnlitColor" 
{
	Properties 
	{
	    _MainTex ("Texture", 2D) = "white" {}
	    _Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
	    _Color ("Color", Color) = (1,1,1,1)
	}
    SubShader 
    {
    	Tags { "Queue" = "AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
    	Cull Off
    	
    	CGPROGRAM
    	#include "UnityCG.cginc"
    	#pragma surface surf Unlit alphatest:_Cutoff
    	
    	struct Input 
    	{
    	    float2 uv_MainTex;
    	};
    	
    	sampler2D _MainTex;
    	uniform float4 _Color;
    	
    	void surf (Input IN, inout SurfaceOutput o) 
    	{
    	    fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
    	    o.Albedo = c.rgb;
    	    o.Alpha = c.a;
    	}
    	
    	half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
  		{
        	return half4(s.Albedo * _LightColor0.rgb * 2, s.Alpha);
   		}
    	ENDCG
    } 
    Fallback "Diffuse"
}
