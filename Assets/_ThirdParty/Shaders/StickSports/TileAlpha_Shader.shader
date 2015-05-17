Shader "Custom/TileAlpha"
{
    Properties
    {
    	_Alpha ("Alpha", Float) = 0.25
    }
    SubShader 
    {
        Tags { "Queue" = "Transparent" }
        
        pass
        {
        	ZWrite Off
         	Blend SrcAlpha OneMinusSrcAlpha 
        	
            CGPROGRAM
            #pragma vertex wfiVertCol
            #pragma fragment passThrough
            #include "UnityCG.cginc"
 
 			float _Alpha;
 
            struct VertOut
            {
                float4 position : POSITION;
                float4 color : COLOR;
            };
 
            struct VertIn
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };
 
            VertOut wfiVertCol(VertIn input, float3 normal : NORMAL)
            {
                VertOut output;
                output.position = mul(UNITY_MATRIX_MVP,input.vertex);
                output.color = input.color;
                return output;
            }
 
            struct FragOut
            {
                float4 color : COLOR;
            };
 
            FragOut passThrough(float4 color : COLOR)
            {
                FragOut output;
                
                output.color = color;
                output.color.a = _Alpha;
                
                return output;
            }
            ENDCG
 
        }
    }
    FallBack "Diffuse"
}