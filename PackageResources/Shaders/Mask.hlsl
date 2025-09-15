#ifndef UNITY_INCLUDE_MASK_INCLUDED
#define UNITY_INCLUDE_MASK_INCLUDED

#include "AppUI.hlsl"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

float _Ratio;

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = float2(v.uv.x, (1.0 - v.uv.y) * (1.0 / _Ratio) );
    return o;
}

float4 _MaskRect;
half4 _InnerMaskColor;
half4 _OuterMaskColor;
float _Radius;
float _Sigma;

half4 frag (v2f i) : SV_Target
{
    const float radius = min(min(_MaskRect.z, _MaskRect.w) * 0.5, _Radius);
    const float2 lower = _MaskRect.xy;
    const float2 upper = _MaskRect.xy + _MaskRect.zw;
    const float sigma = max(0.0001, _Sigma);
    const float shadow = roundedBoxShadow(lower, upper, i.uv, sigma, radius);
    return lerp(_OuterMaskColor, _InnerMaskColor, shadow);
}

#endif // UNITY_INCLUDE_MASK_INCLUDED
