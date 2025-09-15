#ifndef UNITY_APPUI_LINEAR_PROGRESS_INCLUDED
#define UNITY_APPUI_LINEAR_PROGRESS_INCLUDED

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
float _Padding;

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = float2(v.uv.x / (1.0 - _Padding * 2.0) - _Padding, (v.uv.y - 0.5) / _Ratio);
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

inline float circle(float2 uv, float2 pos, float rad)
{
    return 1.0 - smoothstep(rad, rad + _AA, length(uv-pos));
}

half4 frag (v2f i) : SV_Target
{
#if PROGRESS_INDETERMINATE
    const float duration = 1.0;
    const float time = fmod(_Phase.y / duration, 1.0);
    _Start = time < 0.5 ? lerp(0.0, 0.15, time / 0.5) : time < 0.75 ? lerp(0.15, 0.2, (time - 0.5) / 0.25) : lerp(0.2, 0.99, (time - 0.75) / 0.25);
    _End = time < 0.5 ? lerp(0.0, 0.8, time / 0.5) : time < 0.65 ? lerp(0.8, 0.85, (time - 0.5) / 0.15) : time < 0.8 ? lerp(0.85, 1.0, (time - 0.65) / 0.15) : 1.0;
#endif

    // Mask for the value progress
    const float progress = i.uv.x;
    const float radius = 1.0 / _Ratio * 0.5;
    float valueMask = progress >= _Start && progress <= _End ? 1.0 : 0.0;
    const float startCircle = circle(i.uv, float2(_Start, 0), radius) * _Rounded;
    const float endCircle = circle(i.uv, float2(_End, 0), radius) * _Rounded;
    valueMask = max(valueMask, startCircle);
    valueMask = max(valueMask, endCircle);

    half4 color = half4(_Color.rgb, 1);
    color.a *= valueMask;

#ifndef PROGRESS_INDETERMINATE
    // Mask for the buffer progress
    float bufferMask = progress >= _BufferStart && progress <= _BufferEnd ? 1.0 : 0.0;
    const float startBufferCircle = circle(i.uv, float2(_BufferStart, 0), radius) * _Rounded;
    const float endBufferCircle = circle(i.uv, float2(_BufferEnd, 0), radius) * _Rounded;
    bufferMask = max(bufferMask, startBufferCircle);
    bufferMask = max(bufferMask, endBufferCircle);

    color.a = max(color.a, _BufferEnd > 0 ? _BufferOpacity * bufferMask : _BufferOpacity);

#else
    // Make the rest of the bar visible but with low opacity
    color.a = max(color.a, _BufferOpacity);
#endif

    return color;
}

#endif  // UNITY_APPUI_LINEAR_PROGRESS_INCLUDED
