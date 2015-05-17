Shader "Custom/AnimatedWater" 
{
	Properties 
   {
      _Color ("Color", Color) = (1,1,1,1) 
      _MainTex ("_MainTex", 2D) = "white" {}
   }
   SubShader 
   {
  	  Tags {"Queue" = "Geometry"}
  	  
      Pass 
      { 
         CGPROGRAM
         #include "UnityCG.cginc"
         #pragma vertex vert  
         #pragma fragment frag 
		 #pragma exclude_renderers xbox360 ps3 flash
		 
 		 uniform float4 _LightColor0;
 		 
         uniform float4 _Color;
         uniform sampler2D _MainTex;
         float4 _MainTex_ST;
         
         struct vertexInput 
         {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput 
         {
            float4 pos : SV_POSITION;
            float4 col : COLOR;
            float2 uv : TEXCOORD0;
         };
         
         vertexOutput vert(vertexInput input)
         {
            vertexOutput output;            
            
            output.col = _LightColor0 * _Color * 2;
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);
            return output;
         }
         
         float4 frag(vertexOutput input) : COLOR
         {
            return input.col * tex2D(_MainTex, float2(input.uv) * _MainTex_ST.xy + _MainTex_ST.zw);
         }
         
         ENDCG
      }
   }
   Fallback "Diffuse"
}
