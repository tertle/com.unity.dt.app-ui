#ifndef UNITY_APPUI_CIRCULARPROGRESS_INCLUDED
#define UNITY_APPUI_CIRCULARPROGRESS_INCLUDED

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

float _InnerRadius;
int _Rounded;
float _Start;
float _End;
float _BufferStart;
float _BufferEnd;
float _BufferOpacity;
half4 _Color;
float _AA;
float4 _Phase;

inline float2x2 rotate2d(float _angle)
{
    return float2x2(cos(_angle),-sin(_angle),sin(_angle),cos(_angle));
}

inline float circle(float2 uv, float2 pos, float rad)
{
    return 1.0 - smoothstep(rad, rad + _AA, length(uv-pos));
}

float2 getCirclePosition(float progress, float r)
{
    const float rad = lerp(-UNITY_PI, UNITY_PI, progress);
    const float x = r * cos(rad);
    const float y = r * sin(rad);
    return float2(x, y);
}

inline float getProgress(float2 pos)
{
    return inverseLerp(-UNITY_PI, UNITY_PI, atan2(pos.y, pos.x));
}

half4 frag (v2f i) : SV_Target
{
#if PROGRESS_INDETERMINATE
    const float duration = 1.4;
    const float time = fmod(_Phase.y / duration, 1.0);
    i.uv = mul(rotate2d(time * UNITY_TWO_PI), i.uv.xy);
#endif
    const float radius = length(i.uv.xy);
#if PROGRESS_INDETERMINATE
    i.uv = float2(-i.uv.x, i.uv.y);
#else
    i.uv = float2(-i.uv.y, -i.uv.x);
#endif
    const float progress = getProgress(i.uv);

#if PROGRESS_INDETERMINATE
    _Start = time < 0.5 ? lerp(0.0, 0.15, time / 0.5) : time < 0.75 ? lerp(0.15, 0.2, (time - 0.5) / 0.25) : lerp(0.2, 0.99, (time - 0.75) / 0.25);
    _End = time < 0.5 ? lerp(0.0, 0.8, time / 0.5) : time < 0.65 ? lerp(0.8, 0.85, (time - 0.5) / 0.15) : time < 0.8 ? lerp(0.85, 1.0, (time - 0.65) / 0.15) : 1.0;
#endif

    // Mask for the circle itself
    const float thickness = 0.5 - _InnerRadius;
    const float interRadius = _InnerRadius + thickness * 0.5;
    const float mask = smoothstep(0.5, 0.5 - _AA, radius) * smoothstep(_InnerRadius - _AA, _InnerRadius, radius);

    // Mask for the value progress
    float valueMask = progress >= _Start && progress <= _End ? 1.0 : 0.0;
    const float startCircle = circle(i.uv, getCirclePosition(_Start, interRadius), thickness * 0.5) * _Rounded;
    const float endCircle = circle(i.uv, getCirclePosition(_End, interRadius), thickness * 0.5) * _Rounded;
    valueMask = max(valueMask, startCircle);
    valueMask = max(valueMask, endCircle);

    half4 color = half4(_Color.rgb, 1);
    color.a *= valueMask;

#ifndef PROGRESS_INDETERMINATE
    // Mask for the buffer progress
    float bufferMask = progress >= _BufferStart && progress <= _BufferEnd ? 1.0 : 0.0;
    const float startBufferCircle = circle(i.uv, getCirclePosition(_BufferStart, interRadius), thickness * 0.5) * _Rounded;
    const float endBufferCircle = circle(i.uv, getCirclePosition(_BufferEnd, interRadius), thickness * 0.5) * _Rounded;
    bufferMask = max(bufferMask, startBufferCircle);
    bufferMask = max(bufferMask, endBufferCircle);

    color.a = max(color.a, _BufferOpacity * bufferMask);
#endif

    color.a *= mask;
    return color;
}

#endif // UNITY_APPUI_CIRCULARPROGRESS_INCLUDED
