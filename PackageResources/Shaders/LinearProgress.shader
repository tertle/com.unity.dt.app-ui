Shader "Hidden/App UI/LinearProgress"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _InnerRadius ("Inner Radius", Float) = 0
        _Rounded ("Rounded", Int) = 1
        _Start ("Start", Float) = 0
        _End ("End", Float) = 0
        _BufferStart ("Buffer Start", Float) = 0
        _BufferEnd ("Buffer End", Float) = 0
        _BufferOpacity ("Buffer Opacity", Float) = 0.1
        _AA ("Anti-Aliasing", Float) = 0.005
        _Phase("Phase", Vector) = (0,0,0,0)
        _Ratio ("Ratio", Float) = 1.0
        _Padding ("Padding", Float) = 1.0
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
            #pragma multi_compile_local __ PROGRESS_INDETERMINATE
            #include "LinearProgress.hlsl"
            ENDHLSL
        }
    }
}
