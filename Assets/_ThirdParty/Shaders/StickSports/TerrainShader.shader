Shader "Stick Sports/Terrain Shader" {
	Properties
	{
		_MainTint("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SecondTex ("Noise (RGB)", 2D) = "gray" {}
		_SecondIntensity ("Noise intensity", Float) = 1
		_DetailTex ("Detail (RGB)", 2D) = "gray" {}
		_DetailIntensity ("Detail intensity", Float) = 1
		_DetailFocalDistance("Detail focal distance", Float) = 100
		_DetailShortDropOffDistance("Detail near clip distance", Float) = 50
		_DetailLongDropOffDistance("Detail far clip distance", Float) = 150
		_LightMap ("Lightmap", 2D) = "gray" {}
		_LightMapIntensity ("Lightmap intensity", Float) = 1
		
		_ShadowTex ("Shadow Tex", 2D) = "white" {}
		_ShadowTexRot ("Shadow Tex Rotation", Float) = 0
		_ShadowTexRotCenterX ("Shadow Tex Rot Center X", Float) = 0
		_ShadowTexRotCenterY ("Shadow Tex Rot Center Y", Float) = 0
		
		_S1ShadowTexPosX ("S1 Shadow Tex Pos X", Float) = 0
		_S1ShadowTexPosY ("S1 Shadow Tex Pos Y", Float) = 0
		_S1ShadowTexTilingX ("S1 Shadow Tex Tiling X", Float) = 0
		_S1ShadowTexTilingY ("S1 Shadow Tex Tiling Y", Float) = 0
		
		_B1ShadowTexPosX ("B1 Shadow Tex Pos X", Float) = 0
		_B1ShadowTexPosY ("B1 Shadow Tex Pos Y", Float) = 0
		_B1ShadowTexTilingX ("B1 Shadow Tex Tiling X", Float) = 0
		_B1ShadowTexTilingY ("B1 Shadow Tex Tiling Y", Float) = 0
	}
	SubShader
	{
  	  	Tags {"Queue" = "Geometry" "RenderType" = "Opaque" "LightMode" = "ForwardBase"}

      	Pass 
      	{
            Cull Back
            
			CGPROGRAM
         	#include "UnityCG.cginc"
       		#include "AutoLight.cginc"
       		
       		#pragma vertex vert
         	#pragma fragment frag 
            #pragma multi_compile_fwdbase
            #pragma fragmentoption ARB_precision_hint_fastest
            

         	sampler2D _MainTex;
         	sampler2D _SecondTex;
         	sampler2D _DetailTex;
         	sampler2D _LightMap;
         	sampler2D _ShadowTex;
         	
         	float _ShadowTexRot;
         	float _ShadowTexRotCenterX;
         	float _ShadowTexRotCenterY;
         	
         	float _S1ShadowTexPosX;
         	float _S1ShadowTexPosY;
         	float _S1ShadowTexTilingX;
         	float _S1ShadowTexTilingY;
         	
         	float _B1ShadowTexPosX;
         	float _B1ShadowTexPosY;
         	float _B1ShadowTexTilingX;
         	float _B1ShadowTexTilingY;
         	
         	fixed _SecondIntensity;
         	fixed _DetailIntensity;
         	fixed _LightMapIntensity;
         	
         	half _DetailFocalDistance;
         	half _DetailShortDropOffDistance;
         	half _DetailLongDropOffDistance;
         	
         	half4 _MainTint;
          	half4 _MainTex_ST;
         	half4 _SecondTex_ST;
         	half4 _DetailTex_ST;
        	half4 _LightMap_ST;
        	half4 _ShadowTex_ST;
 
         	struct vertexInput 
         	{
            	half4 vertex : POSITION;
            	half4 texcoord : TEXCOORD0;
         	};
         
         	struct vertexOutput 
         	{
            	half4 pos : SV_POSITION;
            	half2 main_uv : TEXCOORD0;
            	half2 second_uv : TEXCOORD1;
            	half2 detail_uv : TEXCOORD2;
            	half2 lightMap_uv : TEXCOORD3;
            	half  worldDistance : TEXCOORD4;
            	float2 s1ShadowTex_uv : TEXCOORD5;
            	float2 b1ShadowTex_uv : TEXCOORD6;
            	//LIGHTING_COORDS(6, 7) 						//This tells it to put the vertex attributes required for lighting into TEXCOORD1 and TEXCOORD2.
         	};

         	vertexOutput vert(appdata_full v)
         	{
            	vertexOutput output;
            
            	output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
            	output.worldDistance = distance(_WorldSpaceCameraPos, mul(_Object2World, v.vertex));
            	output.main_uv = v.texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
            	output.second_uv = v.texcoord * _SecondTex_ST.xy + _SecondTex_ST.zw;
            	output.detail_uv = v.texcoord * _DetailTex_ST.xy + _DetailTex_ST.zw; 
            	output.lightMap_uv = v.texcoord * _LightMap_ST.xy + _LightMap_ST.zw;
            	
            	float2 shadowPos = float2( _S1ShadowTexPosX, _S1ShadowTexPosY );
            	float2 shadow_uv = v.texcoord.xy;
            	shadow_uv -= shadowPos;
            	float sinX = sin ( _ShadowTexRot );
            	float cosX = cos ( _ShadowTexRot );
            	float2x2 rotationMatrix = float2x2( cosX, -sinX, sinX, cosX);
            	shadow_uv = mul ( shadow_uv, rotationMatrix );
            	shadow_uv += shadowPos;
            	output.s1ShadowTex_uv = shadow_uv * float2(_S1ShadowTexTilingX, _S1ShadowTexTilingY) 
            		+ float2((-_S1ShadowTexPosX * _S1ShadowTexTilingX) + _ShadowTexRotCenterX,
            				(-_S1ShadowTexPosY * _S1ShadowTexTilingY) + _ShadowTexRotCenterY);
            	
            	shadowPos = float2( _B1ShadowTexPosX, _B1ShadowTexPosY);
            	shadow_uv = mul ( v.texcoord.xy - shadowPos, rotationMatrix) + shadowPos;
            	output.b1ShadowTex_uv = shadow_uv * float2(_B1ShadowTexTilingX, _B1ShadowTexTilingY) 
            		+ float2((-_B1ShadowTexPosX * _B1ShadowTexTilingX) + _ShadowTexRotCenterX,
            				(-_B1ShadowTexPosY * _B1ShadowTexTilingY) + _ShadowTexRotCenterY);
            																
            	//output.shadowTex_uv = shadow_uv * _ShadowTex_ST.xy + float2((-_ShadowTexPosX * _ShadowTex_ST.x) + _ShadowTexRotCenterX,
                //                                                     		(-_ShadowTexPosY * _ShadowTex_ST.y) + _ShadowTexRotCenterY);
            	//TRANSFER_VERTEX_TO_FRAGMENT(output) //This sets up the vertex attributes required for lighting and passes them through to the fragment shader.
            	return output;
         	}

        	fixed4 frag(vertexOutput i) : COLOR 
         	{
         		//fixed atten = LIGHT_ATTENUATION(i); 	//This gets the shadow and attenuation values combined.
         		
          		fixed4 texColor = tex2D( _MainTex, i.main_uv );
          		
          		fixed4 s1ShadowColor = tex2D( _ShadowTex, i.s1ShadowTex_uv); 
          		fixed4 b1ShadowColor = tex2D( _ShadowTex, i.b1ShadowTex_uv);
          		
          		fixed4 secondTexColor = ( ( tex2D( _SecondTex, i.second_uv ) - 0.5 ) * _SecondIntensity + 0.5 ) * 2.0;
          		
          		half adjusted = i.worldDistance - _DetailFocalDistance;
          		fixed positive = ceil( saturate( adjusted ) );
          		fixed distanceIntensity = 1 - saturate( abs( adjusted ) / ( ( 1 - positive ) * _DetailShortDropOffDistance + positive * _DetailLongDropOffDistance ) );
          		fixed4 detailTexColor = ( ( tex2D( _DetailTex, i.detail_uv ) - 0.5 ) * distanceIntensity * _DetailIntensity + 0.5 ) * 2.0;
          		
          		fixed4 lightMapColor = fixed4( DecodeLightmap( tex2D( _LightMap, i.lightMap_uv ) ), 1.0 );
          		lightMapColor = ( ( lightMapColor - 0.5 ) * _LightMapIntensity + 0.5 ) * 2.0;
          		
            	return _MainTint * s1ShadowColor * b1ShadowColor * texColor * secondTexColor * detailTexColor * lightMapColor; // * atten
         	}
			ENDCG
		}

      	Pass 
      	{
            Cull Front
            Color (0.1,0.1,0,1)
        }
    }

	
	FallBack "Mobile/Diffuse"
}
