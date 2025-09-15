Shader "Hidden/App UI/ColorSwatch"
{
    Properties
    {
        _CheckerColor1 ("Color 1", Color) = (1,1,1,1)
        _CheckerColor2 ("Color 2", Color) = (1,1,1,1)
        _CheckerSize ("Size", Float) = 10
        _Width ("Width", Float) = 200
        _Height ("Height", Float) = 200
        [Toggle] _IsFixed ("Is Fixed", Int) = 0
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
            #include "ColorSwatch_Pass0.hlsl"
            ENDHLSL
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "ColorSwatch_Pass1.hlsl"
            ENDHLSL
        }
    }
}
