#ifndef UNITY_APPUI_COLORSWATCH_PASS1_INCLUDED
#define UNITY_APPUI_COLORSWATCH_PASS1_INCLUDED

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

#define COLORSWATCH_MAX_ITEMS 16

uniform int _Orientation;
uniform int _ColorCount;
uniform int _AlphaCount;
uniform int _IsFixed;
uniform half4 _Colors[COLORSWATCH_MAX_ITEMS];
uniform half4 _Alphas[COLORSWATCH_MAX_ITEMS];

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

half4 frag (v2f i) : SV_Target
{
    const int color_count = _ColorCount;
    const int alpha_count = _AlphaCount;

    if (color_count == 0 || alpha_count == 0)
        return half4(0, 0, 0, 0);

    const float current_position = _Orientation ? i.uv.y : i.uv.x;

    // colors must be sorted by position to work

    int first_color_index = 0;
    int first_alpha_index = 0;

    UNITY_UNROLL
    for (int index = 0; index < COLORSWATCH_MAX_ITEMS; index++)
    {
        if (index < color_count)
            first_color_index = max(first_color_index, step(_Colors[index].w, current_position) * index);

        if (index < alpha_count)
            first_alpha_index = max(first_alpha_index, step(_Alphas[index].y, current_position) * index);

        if (index >= color_count && index >= alpha_count)
            break;
    }

    const int second_color_index = min(first_color_index + 1, color_count - 1);
    const int second_alpha_index = min(first_alpha_index + 1, alpha_count - 1);

    const float colorDelta = (current_position - _Colors[first_color_index].w) / max(0.00001, _Colors[second_color_index].w - _Colors[first_color_index].w);
    const float alphaDelta = (current_position - _Alphas[first_alpha_index].y) / max(0.00001, _Alphas[second_alpha_index].y - _Alphas[first_alpha_index].y);

    const float smoothColorDelta = lerp(smoothstep(0.0, 1.0, colorDelta), step(0.0, colorDelta), _IsFixed);
    const float smoothAlphaDelta = lerp(smoothstep(0.0, 1.0, alphaDelta), step(0.0, alphaDelta), _IsFixed);

    const half3 first_color = _Colors[first_color_index].rgb;
    const half3 second_color = _Colors[second_color_index].rgb;

    const float first_alpha = _Alphas[first_alpha_index].x;
    const float second_alpha = _Alphas[second_alpha_index].x;

    const half3 lerped_color = lerp(first_color, second_color, smoothColorDelta);
    const float lerped_alpha = lerp(first_alpha, second_alpha, smoothAlphaDelta);

    half4 color = half4(lerped_color, lerped_alpha);

#ifdef UNITY_COLORSPACE_GAMMA
    return color;
#else
    return half4(UIGammaToLinear(color.rgb), color.a);
#endif
}

#endif // UNITY_APPUI_COLORSWATCH_PASS1_INCLUDED
