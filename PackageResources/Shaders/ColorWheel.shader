Shader "Hidden/App UI/ColorWheel"
{
    Properties
    {
        _CheckerColor1 ("Color 1", Color) = (1,1,1,1)
        _CheckerColor2 ("Color 2", Color) = (1,1,1,1)
        _CheckerSize ("Size", Float) = 10
        _Width ("Width", Float) = 200
        _Height ("Height", Float) = 200

        _InnerRadius ("Inner Radius", Range(0, 0.5)) = 0.4
        _Saturation ("Saturation", Range(0, 1)) = 1.0
        _Brightness ("Brightness", Range(0,1)) = 1.0
        _Opacity ("Opacity", Range(0,1)) = 1.0
        _AA ("Anti-Aliasing", Float) = 0.005
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        ZWrite Off
        ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "ColorWheel.hlsl"
            ENDHLSL
        }
    }
}
