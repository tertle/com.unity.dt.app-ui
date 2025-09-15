#ifndef UNITY_APPUI_COLORSWATCH_PASS0_INCLUDED
#define UNITY_APPUI_COLORSWATCH_PASS0_INCLUDED

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

half4 _CheckerColor1;
half4 _CheckerColor2;
float _CheckerSize;
float _Width;
float _Height;

v2f vert (appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

half4 frag (v2f i) : SV_Target
{
    half4 color = checker_board(i.uv, _Width, _Height, _CheckerSize, _CheckerColor1, _CheckerColor2);
    return color;
}

#endif // UNITY_APPUI_COLORSWATCH_PASS0_INCLUDED
