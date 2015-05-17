Shader "Lightmap/SeperateLight/Unlit" 
{
   Properties 
   {
      _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
      _MainTex ("Texture", 2D) = "white" {}
      _MapTex ("Lightmap", 2D) = "white" {}
      _NoiseTex ("Noisemap", 2D) = "white" {}
      _CloudAlpha ("Cloud Alpha", Range(0, 1)) = 0
      _LowerLimit ("Lower Limit", Range(0, 1)) = 0
      _ShadowAlpha ("Shadow Alpha", Range(0, 1)) = 0
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
         uniform sampler2D _MapTex;
         uniform sampler2D _NoiseTex;
         float _LowerLimit;
         float4 _MainTex_ST;
         float4 _MapTex_ST;
         float4 _NoiseTex_ST;
         float _CloudAlpha;
         float _ShadowAlpha;
 
         struct vertexInput 
         {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 texcoord : TEXCOORD0;
            float4 texcoord1 : TEXCOORD1;
         };
         struct vertexOutput 
         {
            float4 pos : SV_POSITION;
            float4 col : COLOR;
            float2 uv[3] : TEXCOORD0;
         };
 
         vertexOutput vert(appdata_full input)
         {
            vertexOutput output;
            
            output.col = _Color;
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            output.uv[0] = TRANSFORM_TEX(input.texcoord, _MainTex);
            output.uv[1] = TRANSFORM_TEX(input.texcoord1, _MapTex);
            output.uv[2] = TRANSFORM_TEX(input.texcoord, _NoiseTex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	float mapCol = (max(_LowerLimit, tex2D(_MapTex, float2(input.uv[1]) * _MapTex_ST.xy + _MapTex_ST.zw).r));
         	float noiseCol = (tex2D(_NoiseTex, float2(input.uv[2]) * _NoiseTex_ST.xy + _NoiseTex_ST.zw));
         	mapCol = mapCol + ((1 - mapCol) * (1 - _ShadowAlpha));
         	noiseCol = noiseCol + ((1 - noiseCol) * (1 - _CloudAlpha));
         	
         	float4 textureColor = input.col * tex2D(_MainTex, float2(input.uv[0]) * _MainTex_ST.xy + _MainTex_ST.zw)
         		* mapCol
         		* noiseCol
         		* _LightColor0 * 2;
            return textureColor;
         }
         
         ENDCG
      }
   }
   //Fallback "Diffuse"
}