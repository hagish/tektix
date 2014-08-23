Shader "Unlit/UnlitTransparentColorOnly" {

Properties {
    _Color ("Color", Color) = (1,1,1)
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	Color [_Color]
    Pass {}
} 

}
