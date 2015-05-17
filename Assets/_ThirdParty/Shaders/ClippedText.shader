Shader "Custom/ClippedText"
{

	Properties
	{ 
   		_MainTex ("Font Texture", 2D) = "white" {} 
   		_Color ("Text Color", Color) = (1,1,1,1)
   		
   		_MinPosX ("Min X", Float) = 0
   		_MaxPosX ("Max X", Float) = 0
   		//_MinPosY ("Min Y", Float) = 0
   		//_MaxPosY ("Max Y", Float) = 0
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
      		Lighting Off
      		//Cull On 
      		ZWrite Off 
      		Fog { Mode Off } 
   			Blend SrcAlpha OneMinusSrcAlpha 
      		
      		CGPROGRAM
			
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
	
			//Textures
	        sampler2D _MainTex;
	        uniform float4 _MainTex_ST;
	        
			uniform float4 _Color;
			
			uniform float _MinPosX;
			uniform float _MaxPosX;
			//uniform float _MinPosY;
			//uniform float _MaxPosY;
			
			struct v2f
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
				float4  screenPos : TEXCOORD1;
			};
	
			v2f vert (appdata_full v)
			{
				v2f o;
				
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.screenPos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
	            
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 texColor = tex2D(_MainTex, i.uv.xy);
				
				//Clip the text
				//if (i.screenPos.x < _MinPosX || i.screenPos.x > _MaxPosX || i.screenPos.y < _MinPosY || i.screenPos.y > _MaxPosY)
				if (i.screenPos.x < _MinPosX || i.screenPos.x > _MaxPosX)
				{
					texColor.a = 0;
				}
				else
				{
					texColor.rgb = _Color.rgb;
				}
	            
				return texColor;
			}
			
			ENDCG
   		}
	}
}