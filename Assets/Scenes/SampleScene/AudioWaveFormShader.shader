Shader "Custom/AudioWaveFormShader"
{
    Properties
    {
        _ArraySize ("Array Size", Int) = 256
        _Color ("Color", Color) = (1,1,1,1)
        _BackgroundColor ("BackgroundColor", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            fixed4 _BackgroundColor;

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

            // ComputeBuffer for float arrays with the min and max values of the samples
            StructuredBuffer<float> _MinAmplitude;
            StructuredBuffer<float> _MaxAmplitude;
            int _ArraySize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate the index in the array
                const int index = (int)((1 - i.uv.x) * (_ArraySize - 1));

                // Get the value from the array
                const float minValue = _MinAmplitude[index];
                const float maxValue = _MaxAmplitude[index];

                // Determine whether the the pixel corresponds to an amplitude within the min and max range.
                const bool isWithinRange = i.uv.y >= (minValue / 2 + 0.5) && i.uv.y <= (maxValue / 2 + 0.5);
                fixed4 finalColor = isWithinRange ? _Color : _BackgroundColor;
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
