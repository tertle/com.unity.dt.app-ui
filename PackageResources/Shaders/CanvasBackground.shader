Shader "Hidden/App UI/CanvasBackground"
{
    Properties
    {
        _TexSize("TexSize", Vector) = (1,1,1,1)
        _Thickness("Thickness", Float) = 2.0
        _Spacing("Spacing", Float) = 24.0
        _Color("Color", Color) = (1,1,1,1)
        _Opacity("Opacity", Range(0,1)) = 1.0
        _Scale("Scale", Float) = 1.0
        [Toggle(DRAW_POINTS_ON)] _DrawPoints("Draw Points", Float) = 1
    }
    SubShader
    {
        ZTest Always Cull Off ZWrite Off

        CGINCLUDE

        #include "UnityCG.cginc"

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

        float _Thickness;
        float _Spacing;
        float4 _TexSize;
        float4 _Color;
        float _Opacity;
        float _Scale;

        v2f vert (appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);

            // 1 - reverse the Y axis because UITK uses origin at top-left corner
            // 2 - Take in account the ratio of the screen
            // 3 - Convert to pixel space
            // 4 - Take in account the pan offset
            // 5 - Take in account the zoom factor
            o.uv = (float2(v.uv.x * _TexSize.x, (1.0 - v.uv.y) * _TexSize.y) - _TexSize.zw) / _Scale;
            return o;
        }

        ENDCG

        Pass
        {
            Name "GridBackground"
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local __ DRAW_POINTS_ON

            fixed4 frag (v2f i) : SV_Target
            {
                _Thickness = _Thickness / _Scale;
                // use absolute value for fmod
                i.uv = abs(i.uv);
                const float2 grid = fmod(i.uv, _Spacing);

                const bool insideX = grid.x < _Thickness * 0.5 || (grid.x > (_Spacing - _Thickness * 0.5));
                const bool insideY = grid.y < _Thickness * 0.5 || (grid.y > (_Spacing - _Thickness * 0.5));

#if DRAW_POINTS_ON
                const bool inside = insideX && insideY;
#else // DRAW_LINES_ON
                const bool inside = insideX || insideY;
#endif

                fixed4 color = lerp(fixed4(0,0,0,0), _Color, inside);
                color.a *= _Opacity;

                return color;
            }
            ENDCG
        }
    }
}
