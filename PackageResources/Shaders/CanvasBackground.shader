Shader "Hidden/App UI/CanvasBackground"
{
    Properties
    {
        _TexSize("TexSize", Vector) = (1,1,1,1)
        _Thickness("Thickness", Float) = 2.0
        _Spacing("Spacing", Float) = 24.0
        _Color("Color", Color) = (1,1,1,1)
        _Opacity("Opacity", Range(0,1)) = 1.0
        _Scale("Scale", Float) = 1.0
        [Toggle(DRAW_POINTS_ON)] _DrawPoints("Draw Points", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Always
        Cull Off
        ZWrite Off
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local __ DRAW_POINTS_ON
            #include "CanvasBackground.hlsl"
            ENDHLSL
        }
    }
}
