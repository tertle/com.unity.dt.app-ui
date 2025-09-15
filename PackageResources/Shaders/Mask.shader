Shader "Hidden/App UI/Mask"
{
    Properties
    {
        _InnerMaskColor ("Inner Mask Color", Color) = (0,0,0,1)
        _OuterMaskColor ("Outer Mask Color", Color) = (0,0,0,0)
        _MaskRect ("Mask Rect", Vector) = (0.25,0.25,0.5,0.5)
        _Radius ("Radius", Float) = 0.01
        _Ratio ("Ratio", Float) = 1.77
        _Sigma ("Sigma", Float) = 0.001
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
            #include "Mask.hlsl"
            ENDHLSL
        }
    }
}
