#ifndef UNITY_APPUI_SVSQUARE_INCLUDED
#define UNITY_APPUI_SVSQUARE_INCLUDED

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

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

half4 _Color;

half4 frag (v2f i) : SV_Target
{
    const half4 saturation = lerp(half4(1,1,1,1), half4(_Color.rgb, 1), i.uv.x);
    const float brightness = i.uv.y;
    const half4 color = half4(saturation.rgb * brightness, 1.);

#ifdef UNITY_COLORSPACE_GAMMA
    return color;
#else
    return half4(UIGammaToLinear(color.rgb), color.a);
#endif
}

#endif // UNITY_APPUI_SVSQUARE_INCLUDED
