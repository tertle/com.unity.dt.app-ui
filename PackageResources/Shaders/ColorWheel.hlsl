#ifndef UNITY_APPUI_COLORWHEEL_INCLUDED
#define UNITY_APPUI_COLORWHEEL_INCLUDED

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
    o.uv = v.uv - 0.5;
    return o;
}

half4 _CheckerColor1;
half4 _CheckerColor2;
float _CheckerSize;
float _Width;
float _Height;

float _InnerRadius;
float _Saturation;
float _Brightness;
float _Opacity;
float _AA;

half4 frag (v2f i) : SV_Target
{
    const float radius = length(i.uv.xy);

    const float mask = smoothstep(0.5, 0.5 - _AA, radius) * smoothstep(_InnerRadius - _AA, _InnerRadius, radius);

    const float angle = atan2(i.uv.y, i.uv.x) * UNITY_INV_TWO_PI;

    half4 checker = checker_board(i.uv, _Width, _Height, _CheckerSize, _CheckerColor1, _CheckerColor2);

    half3 hsv = hsv_to_rgb(float3(angle, _Saturation, _Brightness));

#ifndef UNITY_COLORSPACE_GAMMA
    hsv = UIGammaToLinear(hsv);
#endif

    half4 color = lerp(checker, half4(hsv, mask), _Opacity);
    color.a *= mask;
    return color;
}

#endif // UNITY_APPUI_COLORWHEEL_INCLUDED
