Shader "Custom/RageSpline No Fog" {
	Properties {

	}

	Category {
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
		Lighting Off
		Fog { Mode Off }
		BindChannels {
			Bind "Color", color
			Bind "Vertex", vertex
		}
		
		SubShader {
			Pass {
				ZWrite Off
				Cull Off
				Blend SrcAlpha OneMinusSrcAlpha
			}
		}
	}
}
